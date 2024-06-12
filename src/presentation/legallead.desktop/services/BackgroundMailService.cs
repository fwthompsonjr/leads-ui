using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace legallead.desktop.services
{
    internal class BackgroundMailService : IDisposable
    {
        private readonly Timer? _timer;
        private bool disposedValue;

        public BackgroundMailService()
        {

            _timer = new Timer(OnTimer, null,
                TimeSpan.FromMinutes(1d),
                TimeSpan.FromSeconds(45d));
        }


        private void OnTimer(object? state)
        {
            var provider = AppBuilder.ServiceProvider;
            var manager = provider?.GetService<IMailPersistence>();
            var isvalid = false;
            var reader = provider?.GetService<IMailReader>();
            var api = provider?.GetService<IPermissionApi>();
            var user = provider?.GetService<UserBo>();
            try
            {
                if (api == null || user == null || !user.IsAuthenicated) return;
                if (reader == null) return;
                isvalid = true;
                var json = reader.GetCount(api, user);
                if (string.IsNullOrEmpty(json)) return;
                var countBo = ObjectExtensions.TryGet<GetMailCountResponse>(json);
                var messageCount = countBo?.Items ?? 0;
                var messages = manager?.Fetch();
                if (IsCountMatched(messageCount, messages)) return;
                var remote = reader.GetMessages(api, user);
                if (string.IsNullOrEmpty(remote))
                {
                    isvalid = false;
                    return;
                }
                var list = ObjectExtensions.TryGet<List<MailItem>>(remote);
                if (list == null) return;
                manager?.Save(JsonConvert.SerializeObject(list));
            }
            finally
            {
                if (!isvalid) { manager?.Clear(); }
            }
        }

        private static bool IsCountMatched(int messageCount, string? messages)
        {
            if (string.IsNullOrWhiteSpace(messages)) return false;
            var list = ObjectExtensions.TryGet<List<MailItem>>(messages);
            if (list == null) return false;
            return list.Count == messageCount;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer?.Change(Timeout.Infinite, 0);
                    _timer?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
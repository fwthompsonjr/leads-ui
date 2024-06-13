using legallead.desktop.entities;
using legallead.desktop.extensions;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

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

        private bool IsWorking = false;
        private void OnTimer(object? state)
        {
            lock (sync)
            {
                if (IsWorking) { return; }
                IsWorking = true;
                var isvalid = false;
                var provider = AppBuilder.ServiceProvider;
                var manager = provider?.GetService<IMailPersistence>();
                var reader = provider?.GetService<IMailReader>();
                var api = provider?.GetService<IPermissionApi>();
                var user = provider?.GetService<UserBo>();
                try
                {
                    if (api == null || reader == null || user == null || !user.IsAuthenicated) return;
                    var index = VerifyCount(manager, reader, api, user);
                    isvalid = (index == ApiCountMatchedToLocalStorage);
                    if (index != ApiCountGreaterThanStorage) return;
                    var remote = reader.GetMessages(api, user) ?? string.Empty;
                    var list = ObjectExtensions.TryGet<List<MailItem>>(remote);
                    list = FilterByUserId(list, api, user);
                    if (list.Count == 0) return;
                    isvalid = true;
                    var storage = new List<MailStorageItem>();
                    list.ForEach(l =>
                    {
                        string html = GetHTML(l.Id, reader, api, user);
                        if (!string.IsNullOrEmpty(html))
                        {
                            manager?.Save(l.Id ?? string.Empty, html);
                        }
                        storage.Add(l.ToStorage());
                    });
                    manager?.Save(JsonConvert.SerializeObject(storage));
                }
                finally
                {
                    IsWorking = false;
                    if (!isvalid) { manager?.Clear(); }
                }

            }
        }

        private static string GetHTML(string? id, IMailReader reader, IPermissionApi api, UserBo user)
        {
            if (string.IsNullOrEmpty(id)) return string.Empty;
            var json = reader.GetBody(api, user, id); if (string.IsNullOrEmpty(json)) return string.Empty;
            var bo = ObjectExtensions.TryGet<GetMailBodyResponse>(json);
            if (bo == null) return string.Empty;
            return bo.Body ?? string.Empty;
        }

        private static int VerifyCount(IMailPersistence? manager, IMailReader reader, IPermissionApi api, UserBo user)
        {
            var json = reader.GetCount(api, user);
            if (string.IsNullOrEmpty(json)) return ApiReturnedNullResponse;
            var countBo = ObjectExtensions.TryGet<GetMailCountResponse>(json);
            var messageCount = countBo?.Items ?? 0;
            var messages = manager?.Fetch();
            if (IsCountMatched(messageCount, messages)) return ApiCountMatchedToLocalStorage;
            return ApiCountGreaterThanStorage;
        }

        private static List<MailItem> FilterByUserId(List<MailItem> source, IPermissionApi api, UserBo user)
        {
            try
            {
                var list = new List<MailItem>();
                list.AddRange(source.FindAll(x => !string.IsNullOrEmpty(x.Id)));
                var userid = user.GetUserId(api).GetAwaiter().GetResult() ?? string.Empty;
                list.RemoveAll(x => !userid.Equals(x.Id ?? string.Empty));
                return list;
            }
            catch (Exception)
            {
                return new();
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

        private static readonly object sync = new();
        private const int ApiReturnedNullResponse = -1;
        private const int ApiCountMatchedToLocalStorage = 10;
        private const int ApiCountGreaterThanStorage = 100;
    }
}
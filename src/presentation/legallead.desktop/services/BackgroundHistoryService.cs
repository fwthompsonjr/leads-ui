﻿using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace legallead.desktop.services
{
    internal class BackgroundHistoryService : IDisposable
    {
        private readonly Timer? _timer;
        private bool disposedValue;

        public BackgroundHistoryService()
        {

            _timer = new Timer(OnTimer, null,
                TimeSpan.FromSeconds(10d),
                TimeSpan.FromSeconds(15d));
        }

        private bool IsWorking = false;

        internal void OnTimer(object? state)
        {
            lock (sync)
            {
                if (IsWorking) { return; }
                IsWorking = true;
                var isvalid = false;
                var provider = AppBuilder.ServiceProvider;
                var manager = provider?.GetService<IHistoryPersistence>();
                var api = provider?.GetService<IPermissionApi>();
                var user = provider?.GetService<UserBo>();
                try
                {
                    if (api == null || user == null || !user.IsAuthenicated || user.Applications == null) return;
                    CheckPermission(api, user);
                    isvalid = GetHistory(manager, api, user, user.Applications[0]);
                    isvalid &= GetRestriction(manager, api, user, user.Applications[0]);
                }
                finally
                {
                    SetEnabled(isvalid);
                    if (isvalid)
                    {
                        // change time to poll every 5 minutes
                        _timer?.Change(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));
                    }
                    else
                    {
                        // change time to poll every 30 seconds when user is logged out
                        manager?.Clear();
                        _timer?.Change(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
                    }
                    IsWorking = false;
                }

            }
        }

        private static void CheckPermission(IPermissionApi api, UserBo user)
        {
            const string dash = "-";
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher == null) { return; }
            Window mainWindow = dispatcher.Invoke(() =>
            {
                return Application.Current.MainWindow;
            });
            if (mainWindow is not MainWindow main) return;
            dispatcher.InvokeAsync(async () =>
            {
                var permissions = await api.Get("user-permissions-list", user);
                if (permissions == null || permissions.StatusCode != 200)
                {
                    main.sbUserLevelText.Text = dash;
                    return;
                }
                var mapped = ObjectExtensions.TryGet<List<ContactPermissionResponse>>(permissions.Message);
                var current = mapped.Find(x => x.KeyName.Equals("Account.Permission.Level"));
                var level = current?.KeyValue?.ToUpper() ?? dash;
                if (main.sbUserLevelText.Text == level) return;
                main.sbUserLevelText.Text = level;
            });
        }

        private static bool GetHistory(IHistoryPersistence? manager, IPermissionApi api, UserBo user, object payload)
        {
            try
            {
                var stuff = api.Post("search-get-history", payload, user).Result;
                if (stuff.StatusCode != 200) return false;
                var list = ObjectExtensions.TryGet<List<UserSearchQueryBo>>(stuff.Message);
                if (list.Count == 0) return false;
                list.ForEach(x =>
                {
                    if (x.CreateDate.HasValue)
                    {
                        var dt = DateTime.SpecifyKind(x.CreateDate.Value, DateTimeKind.Utc).ToLocalTime();
                        x.CreateDate = dt;
                    }
                });
                manager?.Save(JsonConvert.SerializeObject(list));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool GetRestriction(IHistoryPersistence? manager, IPermissionApi api, UserBo user, object payload)
        {
            try
            {
                var stuff = api.Post("search-get-restriction", payload, user).Result;
                if (stuff.StatusCode != 200) return false;
                var list = ObjectExtensions.TryGet<MySearchRestrictions>(stuff.Message);
                manager?.SaveRestriction(JsonConvert.SerializeObject(list));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void SetEnabled(bool isvalid)
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher == null) { return; }
            Window mainWindow = dispatcher.Invoke(() =>
            {
                return Application.Current.MainWindow;
            });
            if (mainWindow is not MainWindow main) return;
            dispatcher.Invoke(() =>
            {
                var menuItem = main.mnuMySearchProfile;
                if (menuItem != null)
                {
                    menuItem.IsEnabled = isvalid;
                }
            });
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
    }
}
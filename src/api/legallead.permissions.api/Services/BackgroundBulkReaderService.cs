
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using page.load.utility;
using page.load.utility.Entities;
using page.load.utility.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;

namespace legallead.permissions.api.Services
{
    public class BackgroundBulkReaderService(IFetchDbAddress addressService, IUserUsageService usageService) : IStartupTask
    {
        private readonly IFetchDbAddress _addressSvc = addressService;
        private readonly IUserUsageService _usageService = usageService;
        private readonly static List<BulkReadResponse> offlineRequests = [];
        private Timer? _timer;
        private static BackgroundBulkReaderService? service;
        private static void OnTimedHandler(object? state)
        {
            if (isRunning) return;
            if (state is not BackgroundBulkReaderService svc) return;
            lock (locker)
            {
                isRunning = true;
                service ??= svc;
            }
            try
            {
                _ = Task.Run(async () => {
                    var usageDb = svc._usageService;
                    var addressSvc = svc._addressSvc;
                    var queue = await usageDb.GetOfflineWorkQueueAsync();
                    if (queue == null || queue.Count == 0) return;
                    var list = queue.ToList();
                    var working = offlineRequests.Select(x => x.OfflineRequestId);
                    list.RemoveAll(x => string.IsNullOrEmpty(x.RequestId));
                    list.RemoveAll(x => string.IsNullOrEmpty(x.Cookie));
                    list.RemoveAll(x => string.IsNullOrEmpty(x.Workload));
                    list.RemoveAll(x => working.Contains(x.OfflineId));
                    if (list.Count == 0) return;
                    foreach (var request in list)
                    {
                        var response = new BulkReadResponse { RequestId = request.RequestId };
                        var settings = request.Cookie.ToInstance<List<CookieModel>>();
                        var items = request.Workload.ToInstance<List<CaseItemDto>>();
                        if (settings == null || items == null) continue;
                        var model = new OfflineDataModel
                        {
                            RequestId = request.RequestId,
                            Cookie = request.Cookie,
                            Workload = request.Workload,
                            RowCount = items.Count
                        };
                        model = await usageDb.AppendOfflineRecordAsync(model);
                        var cookies = settings.Select(s => s.Cookie()).ToList();
                        response.IsValid = true;
                        var objRequestGuid =
                            string.IsNullOrEmpty(model.OfflineId) ?
                            Guid.NewGuid().ToString("D") : model.OfflineId;
                        response.OfflineRequestId = objRequestGuid;
                        offlineRequests.Add(response);
                        var reader = new BulkCaseReader(
                            new ReadOnlyCollection<OpenQA.Selenium.Cookie>(cookies),
                            items,
                            new BulkReadMessages { OfflineRequestId = objRequestGuid, AddressSerice = addressSvc })
                        {
                            OnStatusUpdated = StatusChanged,
                            OnStatusTimeOut = StatusTerminated
                        };
                        _ = Task.Run(() =>
                        {
                            var rsp = reader.Execute();
                            if (rsp is not string json) return;
                            var find = offlineRequests.FirstOrDefault(x => x.OfflineRequestId == response.OfflineRequestId);
                            if (find != null)
                            {
                                find.IsCompleted = true;
                                find.Content = json;
                            }
                        });
                    }
                });
            }
            finally
            {
                lock (locker)
                {
                    isRunning = false; 
                }
            }
        }

        public int Index => 100;

        public Task ExecuteAsync()
        {
            if (_timer == null) _ = GetTimer();
            return Task.CompletedTask;
        }

        private Timer GetTimer()
        {
            if (_timer != null) return _timer;
            _timer = new Timer(OnTimedHandler, this, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));
            
            return _timer;
        }

        private static void StatusChanged(object? sender, BulkReadMessages e)
        {
            if (service == null) return;
            var find = offlineRequests.FirstOrDefault(x => x.OfflineRequestId == e.OfflineRequestId);
            if (find == null) return;
            find.TotalProcessed = e.TotalProcessed;
            find.RecordCount = e.RecordCount;
            find.Messages.Clear();
            find.Messages.AddRange(e.Messages);
            if (string.IsNullOrEmpty(e.Workload) || e.TotalProcessed == 0) { return; }
            var isCompleted = e.TotalProcessed == e.RecordCount && e.RecordCount > 0;
            var model = new OfflineDataModel
            {
                RequestId = find.RequestId, // e.OfflineRequestId,
                Message = string.Join(Environment.NewLine, e.Messages),
                Workload = e.Workload,
                RowCount = isCompleted ? e.TotalProcessed - 1 : e.TotalProcessed,
                RetryCount = e.RetryCount,
            };
            _ = Task.Run(async () =>
            {
                await service._usageService.UpdateOfflineRecordAsync(model);
                if (isCompleted)
                {
                    model.RowCount = e.TotalProcessed;
                    await service._usageService.UpdateOfflineRecordAsync(model);
                    await service._usageService.UpdateOfflineHistoryAsync();
                }
            });
        }

        private static void StatusTerminated(object? sender, BulkReadMessages e)
        {
            if (service == null) return;
            var messages = new string[] {
                "Process terminated due to server timeout",
                $"Total records processed: {e.TotalProcessed}",
                $"Total retries {e.RetryCount}"
            };
            var model = new OfflineDataModel
            {
                RequestId = e.OfflineRequestId,
                Message = string.Empty,
                Workload = string.Join(Environment.NewLine, messages),
                RowCount = e.TotalProcessed,
                RetryCount = e.RetryCount,
            };
            _ = Task.Run(async () =>
            {
                await service._usageService.TerminateOfflineRequestAsync(model);
            });
        }
        private static bool isRunning = false;
        private readonly static object locker = new ();

        private class CookieModel
        {

            public string Name { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Domain { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string SameSite { get; set; } = string.Empty;
            public string Expiry { get; set; } = string.Empty;

            public OpenQA.Selenium.Cookie Cookie()
            {
                var hasDate = DateTime.TryParse(Expiry, CultureInfo.CurrentCulture, out var date);
                DateTime? expirationDt = hasDate ? date : null;
                return new OpenQA.Selenium.Cookie(Name, Value, Domain, Path, expirationDt, false, false, SameSite);
            }
        }
    }
}

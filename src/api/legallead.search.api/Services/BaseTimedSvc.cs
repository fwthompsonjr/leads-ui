using legallead.jdbc.interfaces;
using Newtonsoft.Json;
using System.Diagnostics;

namespace legallead.search.api.Services
{
    internal abstract class BaseTimedSvc<T> : IHostedService, IDisposable where T : class
    {
        protected readonly ILoggingRepository _logger;
        protected readonly ISearchQueueRepository _queueDb;
        protected readonly Svcs DataService;

        private Timer? _timer = null;
        private bool disposedValue;
        protected readonly object _lock = new();
        protected virtual int DelayedStartInSeconds { get; set; } = 15;
        protected virtual int IntervalInMinutes { get; set; } = 15;

        public bool IsWorking { get; protected set; }

        protected BaseTimedSvc(ILoggingRepository logger, ISearchQueueRepository repo)
        {
            _logger = logger;
            _queueDb = repo;
            DataService = new Svcs(logger);
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            var message = $"{typeof(T).Name} : {DateTime.Now:s} : Timed Process is starting";
            Console.WriteLine(message);
            _timer = new Timer(OnTimer, null, TimeSpan.FromSeconds(DelayedStartInSeconds), TimeSpan.FromMinutes(IntervalInMinutes));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            var message = $"{typeof(T).Name} : {DateTime.Now:s} : Timed Process is stopping";
            Console.WriteLine(message);

            _timer?.Change(Timeout.Infinite, 0);
            var processes = new List<string> {
                "geckodriver",
                "chromedriver",
                "IEDriverServer" };
            processes.ForEach(Kill);
            return Task.CompletedTask;
        }

        protected abstract void DoWork(object? state);

        private void OnTimer(object? state)
        {
            // write service status here
            var componentName = typeof(T).Namespace ?? "namespace.undefined";
            var typeName = typeof(T).Name;
            var isActive = DataService.GetStatus(componentName, typeName);
            DataService.ReportState(componentName, typeName, isActive);
            if (isActive) DoWork(state);
        }
        private static void Kill(string processName)
        {
            var enumerable = Process.GetProcessesByName(processName);
            if (enumerable == null || !enumerable.Any()) return;
            foreach (var process in enumerable)
            {
                process.Kill();
            }
        }

        protected class Svcs
        {
            private readonly ILoggingRepository _logging;

            public Svcs(ILoggingRepository logging)
            {
                _logging = logging;
            }
            /// <summary>
            ///	There is a table in the repository that stores the on/off state
            /// of each service. This method queries the database
            /// to determine if the service is active.
            /// </summary>
            public bool GetStatus(string componentName, string typeName)
            {
                Console.WriteLine("GetStatus: {0}, {1}", componentName, typeName);
                return true;
            }

            internal void ReportState(string componentName, string typeName, bool isActive)
            {
                var log = new
                {
                    Environment.MachineName,
                    ComponentType = componentName,
                    ProcessName = typeName,
                    IsActive = isActive,
                    DateCreated = DateTime.UtcNow
                };
                var serial = JsonConvert.SerializeObject(log);
                Console.WriteLine(serial);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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

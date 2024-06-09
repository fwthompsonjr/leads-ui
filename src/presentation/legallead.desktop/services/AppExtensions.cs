using legallead.desktop.services;
using System;
using System.Threading;

namespace legallead.desktop
{
    public partial class App : IDisposable
    {
        private readonly BackgroundQueueServices backgroundQueue;
        private bool disposedValue;

        public App()
        {
            using var source = new CancellationTokenSource();
            backgroundQueue = new BackgroundQueueServices();
            backgroundQueue.StartAsync(source.Token);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using var source = new CancellationTokenSource();
                    backgroundQueue.StopAsync(source.Token);
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

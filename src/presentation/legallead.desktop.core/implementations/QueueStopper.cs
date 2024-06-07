using legallead.desktop.interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.implementations
{
    internal class QueueStopper : IQueueStopper
    {
        private readonly IQueueSettings queueSettings;
        public QueueStopper(IQueueSettings settings)
        { 
            queueSettings = settings;
        }

        public string ServiceName => queueSettings.Name ?? string.Empty;

        public void Stop()
        {
            lock (sync)
            {
                if (!queueSettings.IsEnabled) { return; }
                if (string.IsNullOrWhiteSpace(ServiceName)) { return; }

                var processes = Process.GetProcessesByName(ServiceName).ToList();
                if (processes.Count == 0) { return; }
                processes.ForEach(p => { p.Kill(); }); 
            }
        }

        private static readonly object sync = new();
    }
}
/*

*/
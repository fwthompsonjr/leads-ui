using legallead.desktop.interfaces;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace legallead.desktop.implementations
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with file system. Tested in integration only")]
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
                processes.ForEach(p =>
                {
                    var indicationModel = GetIndicationModel(p);
                    if (indicationModel == null || !indicationModel.IsWorking) { p.Kill(); }
                    else { SendStopNotification(indicationModel); }
                });
            }
        }
        /// <summary>
        /// Toggles ShutdownOnComplete attribute of file and saves to service _sys directory.
        /// This tells the service that it can exit when queue processing work is completed.
        /// </summary>
        /// <param name="indicationModel"></param>
        private static void SendStopNotification(IndicationModel indicationModel)
        {
            var fullPath = indicationModel.FileLocation;
            if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath)) { return; }
            var copy = new IndicationModel { IsWorking = indicationModel.IsWorking, ShutdownOnComplete = true };
            var json = JsonConvert.SerializeObject(copy, Formatting.Indented);
            File.Delete(fullPath);
            File.WriteAllText(fullPath, json);
        }
        /// <summary>
        /// Retrieves copy of service state information, if available
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private static IndicationModel? GetIndicationModel(Process process)
        {
            try
            {
                var fullPath = process.MainModule?.FileName;
                if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath)) { return default; }
                var parent = Path.GetDirectoryName(fullPath);
                if (string.IsNullOrWhiteSpace(parent) || !Directory.Exists(parent)) { return default; }
                var child = Path.Combine(parent, "_sys");
                if (string.IsNullOrWhiteSpace(child) || !Directory.Exists(child)) { return default; }
                var stateFile = Path.Combine(child, "process-state.json");
                if (string.IsNullOrWhiteSpace(stateFile) || !File.Exists(stateFile)) { return default; }
                string content = GetFileContent(stateFile);
                if (string.IsNullOrWhiteSpace(content) || !File.Exists(content)) { return default; }
                var model = GetModel(content);
                if (model == null) { return default; }
                model.FileLocation = stateFile;
                return model;

            }
            catch (Exception)
            {
                return default;
            }
        }

        private static string GetFileContent(string stateFile)
        {
            try
            {
                using var reader = new StreamReader(stateFile);
                return reader.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }
        private static IndicationModel? GetModel(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<IndicationModel>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private sealed class IndicationModel
        {
            [JsonProperty("isWorking")]
            public bool IsWorking { get; set; }
            [JsonProperty("autoShutdown")]
            public bool ShutdownOnComplete { get; set; }
            public string? FileLocation { get; set; }
        }

        private static readonly object sync = new();
    }
}
/*

*/
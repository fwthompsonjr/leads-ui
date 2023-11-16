using System.Diagnostics;

namespace legallead.records.search.Db
{
    [DataAction(Name = "header", ProcessId = 1000, IsShared = true)]
    public class WebCleanUpAction : BaseAction
    {
        public WebCleanUpAction(HccProcess process) : base(process)
        {
        }

        public override TimeSpan EstimatedDuration => TimeSpan.FromSeconds(2);

        public override void Execute(IProgress<HccProcess> progress)
        {
            ReportProgress = progress;
            Start();
            CleanUp();
            End();
        }

        private static void CleanUp()
        {
            try
            {
                KillChrome();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                KillProcess("chromedriver");
            }
        }

        private static void KillChrome()
        {
            IEnumerable<Process> processes = Process.GetProcessesByName("chrome")
                .Where(_ => !_.MainWindowHandle.Equals(IntPtr.Zero));
            foreach (Process? process in processes)
            {
                process.Kill();
            }
        }

        private static void KillProcess(string processName)
        {
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
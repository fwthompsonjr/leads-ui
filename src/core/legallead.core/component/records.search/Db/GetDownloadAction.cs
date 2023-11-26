using legallead.records.search.Web;

namespace legallead.records.search.Db
{
    [DataAction(Name = "header", ProcessId = 2)]
    public class GetDownloadAction : BaseAction
    {
        public GetDownloadAction(HccProcess process) : base(process)
        {
        }

        public override TimeSpan EstimatedDuration => TimeSpan.FromMinutes(3);

        public override void Execute(IProgress<HccProcess> progress)
        {
            ReportProgress = progress;
            Start();
            string fileName = GetDownload();
            Information($"File {fileName}. Downloaded");
            End();
        }

        private string GetDownload()
        {
            using HarrisCriminalData obj = new();
            var result = obj.GetData(WebDriver);
            return result ?? string.Empty;
        }
    }
}
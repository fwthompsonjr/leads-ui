// GetCaseDetailAction
using legallead.records.search.Dto;
using legallead.records.search.Web;
using Newtonsoft.Json;

namespace legallead.records.search.Db
{
    [DataAction(Name = "detail", ProcessId = 2)]
    public class GetCaseDetailAction : BaseAction
    {
        public GetCaseDetailAction(HccProcess process) : base(process)
        {
        }

        public override TimeSpan EstimatedDuration => TimeSpan.FromMinutes(3);

        public override void Execute(IProgress<HccProcess> progress)
        {
            DateTime MxDate = DateTime.Now.AddDays(-1).Date;
            DateTime MnDate = MxDate.AddDays(GetOptionValue());

            ReportProgress = progress;
            Start();
            List<HarrisCriminalStyleDto> fileName = GetDownload(MnDate, MxDate);
            Information($"File {fileName}. Downloaded");
            End();
        }

        private int GetOptionValue()
        {
            HccOptionDto data = GetOption();
            List<string> list = data.Values.ToList();
            int listId = data.Index.GetValueOrDefault(0);
            int indexId = Convert.ToInt32(list[listId], CultureInfo.CurrentCulture);
            return -1 * indexId;
        }

        private static HccOptionDto GetOption()
        {
            string data = DataOptions.Read();
            HccOptionDto? list = JsonConvert.DeserializeObject<List<HccOptionDto>>(data)
                .Where(a => a.Type.Equals("settings", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(b => b.Id.Equals(110));
            return list;
        }

        private List<Dto.HarrisCriminalStyleDto> GetDownload(DateTime mnDate, DateTime mxDate)
        {
            using HarrisCriminalCaseStyle obj = new();
            List<HarrisCriminalStyleDto> records = obj.GetCases(WebDriver, mnDate, mxDate);
            return records;
        }
    }
}
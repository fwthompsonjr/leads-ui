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
            var fileName = GetDownload(MnDate, MxDate);
            Information($"File {fileName}. Downloaded");
            End();
        }

        private int GetOptionValue()
        {
            var data = GetOption();
            var list = data.Values.ToList();
            var listId = data.Index.GetValueOrDefault(0);
            var indexId = Convert.ToInt32(list[listId], CultureInfo.CurrentCulture);
            return -1 * indexId;
        }

        private HccOptionDto GetOption()
        {
            var data = DataOptions.Read();
            var list = JsonConvert.DeserializeObject<List<HccOptionDto>>(data)
                .Where(a => a.Type.Equals("settings", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(b => b.Id.Equals(110));
            return list;
        }

        private List<Dto.HarrisCriminalStyleDto> GetDownload(DateTime mnDate, DateTime mxDate)
        {
            using var obj = new HarrisCriminalCaseStyle();
            var records = obj.GetCases(WebDriver, mnDate, mxDate);
            return records;
        }
    }
}
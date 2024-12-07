using legallead.jdbc.interfaces;
using Newtonsoft.Json;
using System.Text;

namespace legallead.permissions.api.Services
{
    public class HccRecordLoadingService(
    string encodedData,
    IHarrisLoadRepository harrisdb,
    int maxUploadSize = 500)
    {
        private readonly string _encodedData = encodedData;
        private readonly int mxcount = maxUploadSize;
        private readonly IHarrisLoadRepository _hccDataService = harrisdb;

        public async Task LoadAsync()
        {
            if (string.IsNullOrEmpty(_encodedData))
                throw new ArgumentOutOfRangeException("encodedData");
            var array = Convert.FromBase64String(_encodedData);
            var decoded = Encoding.UTF8.GetString(array);
            var dataset = decoded.Split(
                Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (dataset.Count < 2) return;
            var subset = dataset.GetRange(1, dataset.Count - 1);
            var list = new List<UploadLineItem>();
            foreach (var s in subset)
            {
                var values = s.Split('\t').ToList();
                list.Add(new UploadLineItem
                {
                    CourtDivisionIndicator = TryGetValue(values, 0),
                    CaseNumber = TryGetValue(values, 1),
                    CaseFileDate = TryGetValue(values, 2),
                    InstrumentType = TryGetValue(values, 3),
                    SettingResults = TryGetValue(values, 4),
                    CourtNumber = TryGetValue(values, 5),
                    CaseStatus = TryGetValue(values, 6),
                    DefendantStatus = TryGetValue(values, 7),
                    BondAmount = TryGetValue(values, 8),
                    CurrentOffenseCode = TryGetValue(values, 9),
                    CurrentOffenseLiteral = TryGetValue(values, 10),
                    CurrentOffenseLevelAndDegree = TryGetValue(values, 11),
                    ComplaintOffenseCode = TryGetValue(values, 12),
                    ComplaintOffenseLiteral = TryGetValue(values, 13),
                    ComplaintOffenseLevelAndDegree = TryGetValue(values, 14),
                    GrandJuryOffenseCode = TryGetValue(values, 15),
                    GrandJuryOffenseLiteral = TryGetValue(values, 16),
                    GrandJuryOffenseLevelAndDegree = TryGetValue(values, 17),
                    NextAppearanceDate = TryGetValue(values, 18),
                    DocketType = TryGetValue(values, 19),
                    NextAppearanceReason = TryGetValue(values, 20),
                    DefendantName = TryGetValue(values, 21),
                    DefendantSpn = TryGetValue(values, 22),
                    DefendantRace = TryGetValue(values, 23),
                    DefendantSex = TryGetValue(values, 24),
                    DefendantDateOfBirth = TryGetValue(values, 25),
                    DefendantStreetNumber = TryGetValue(values, 26),
                    DefendantStreetName = TryGetValue(values, 27),
                    DefendantApartmentNumber = TryGetValue(values, 28),
                    DefendantCity = TryGetValue(values, 29),
                    DefendantState = TryGetValue(values, 30),
                    DefendantZip = TryGetValue(values, 31),
                    AttorneyName = TryGetValue(values, 32),
                    AttorneySpn = TryGetValue(values, 33),
                    AttorneyConnectionCode = TryGetValue(values, 34),
                    AttorneyConnectionLiteral = TryGetValue(values, 35),
                    ComplainantName = TryGetValue(values, 36),
                    ComplainantAgency = TryGetValue(values, 37),
                    OffenseReportNumber = TryGetValue(values, 38),
                    DispositionDate = TryGetValue(values, 39),
                    Disposition = TryGetValue(values, 40),
                    Sentence = TryGetValue(values, 41),
                    DefCitizenshipStatus = TryGetValue(values, 42),
                    BondException = TryGetValue(values, 43),
                    GrandJuryDate = TryGetValue(values, 44),
                    GrandJuryCourt = TryGetValue(values, 45),
                    GrandJuryAction = TryGetValue(values, 46),
                    DefendantPlaceOfBirth = TryGetValue(values, 47)
                });
                if (list.Count >= mxcount)
                {
                    var js = JsonConvert.SerializeObject(list);
                    await _hccDataService.Append(js);
                    list.Clear();
                }
            }
            if (list.Count == 0) return;
            var jsfinal = JsonConvert.SerializeObject(list);
            await _hccDataService.Append(jsfinal);
        }

        private static string TryGetValue(List<string> list, int index)
        {
            if (index < 0) return string.Empty;
            if (index > list.Count - 1) return string.Empty;
            return list[index];
        }

        private sealed class UploadLineItem
        {
            [JsonProperty("cdi")] public string CourtDivisionIndicator { get; set; } = string.Empty;
            [JsonProperty("cas")] public string CaseNumber { get; set; } = string.Empty;
            [JsonProperty("fda")] public string CaseFileDate { get; set; } = string.Empty;
            [JsonProperty("ins")] public string InstrumentType { get; set; } = string.Empty;
            [JsonProperty("cad")] public string SettingResults { get; set; } = string.Empty;
            [JsonProperty("crt")] public string CourtNumber { get; set; } = string.Empty;
            [JsonProperty("cst")] public string CaseStatus { get; set; } = string.Empty;
            [JsonProperty("dst")] public string DefendantStatus { get; set; } = string.Empty;
            [JsonProperty("bam")] public string BondAmount { get; set; } = string.Empty;
            [JsonProperty("curr_off")] public string CurrentOffenseCode { get; set; } = string.Empty;
            [JsonProperty("curr_off_lit")] public string CurrentOffenseLiteral { get; set; } = string.Empty;
            [JsonProperty("curr_l_d")] public string CurrentOffenseLevelAndDegree { get; set; } = string.Empty;
            [JsonProperty("com_off")] public string ComplaintOffenseCode { get; set; } = string.Empty;
            [JsonProperty("com_off_lit")] public string ComplaintOffenseLiteral { get; set; } = string.Empty;
            [JsonProperty("com_l_d")] public string ComplaintOffenseLevelAndDegree { get; set; } = string.Empty;
            [JsonProperty("gj_off")] public string GrandJuryOffenseCode { get; set; } = string.Empty;
            [JsonProperty("gj_off_lit")] public string GrandJuryOffenseLiteral { get; set; } = string.Empty;
            [JsonProperty("gj_l_d")] public string GrandJuryOffenseLevelAndDegree { get; set; } = string.Empty;
            [JsonProperty("nda")] public string NextAppearanceDate { get; set; } = string.Empty;
            [JsonProperty("cnc")] public string DocketType { get; set; } = string.Empty;
            [JsonProperty("rea")] public string NextAppearanceReason { get; set; } = string.Empty;
            [JsonProperty("def_nam")] public string DefendantName { get; set; } = string.Empty;
            [JsonProperty("def_spn")] public string DefendantSpn { get; set; } = string.Empty;
            [JsonProperty("def_rac")] public string DefendantRace { get; set; } = string.Empty;
            [JsonProperty("def_sex")] public string DefendantSex { get; set; } = string.Empty;
            [JsonProperty("def_dob")] public string DefendantDateOfBirth { get; set; } = string.Empty;
            [JsonProperty("def_stnum")] public string DefendantStreetNumber { get; set; } = string.Empty;
            [JsonProperty("def_stnam")] public string DefendantStreetName { get; set; } = string.Empty;
            [JsonProperty("def_apt")] public string DefendantApartmentNumber { get; set; } = string.Empty;
            [JsonProperty("def_cty")] public string DefendantCity { get; set; } = string.Empty;
            [JsonProperty("def_st")] public string DefendantState { get; set; } = string.Empty;
            [JsonProperty("def_zip")] public string DefendantZip { get; set; } = string.Empty;
            [JsonProperty("aty_nam")] public string AttorneyName { get; set; } = string.Empty;
            [JsonProperty("aty_spn")] public string AttorneySpn { get; set; } = string.Empty;
            [JsonProperty("aty_coc")] public string AttorneyConnectionCode { get; set; } = string.Empty;
            [JsonProperty("aty_coc_lit")] public string AttorneyConnectionLiteral { get; set; } = string.Empty;
            [JsonProperty("comp_nam")] public string ComplainantName { get; set; } = string.Empty;
            [JsonProperty("comp_agency")] public string ComplainantAgency { get; set; } = string.Empty;
            [JsonProperty("off_rpt_num")] public string OffenseReportNumber { get; set; } = string.Empty;
            [JsonProperty("dispdt")] public string DispositionDate { get; set; } = string.Empty;
            [JsonProperty("disposition")] public string Disposition { get; set; } = string.Empty;
            [JsonProperty("sentence")] public string Sentence { get; set; } = string.Empty;
            [JsonProperty("def_citizen")] public string DefCitizenshipStatus { get; set; } = string.Empty;
            [JsonProperty("bamexp")] public string BondException { get; set; } = string.Empty;
            [JsonProperty("gj_dt")] public string GrandJuryDate { get; set; } = string.Empty;
            [JsonProperty("gj_crt")] public string GrandJuryCourt { get; set; } = string.Empty;
            [JsonProperty("gj_cdp")] public string GrandJuryAction { get; set; } = string.Empty;
            [JsonProperty("def_pob")] public string DefendantPlaceOfBirth { get; set; } = string.Empty;


        }
    }
}
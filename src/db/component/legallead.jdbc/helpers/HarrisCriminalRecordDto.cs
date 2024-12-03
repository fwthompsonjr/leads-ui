using Newtonsoft.Json;

namespace legallead.jdbc.helpers
{
    internal class HarrisCriminalRecordDto
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

        public string this[int index]
        {
            get
            {
                if (index == 0) return CourtDivisionIndicator;
                if (index == 1) return CaseNumber;
                if (index == 2) return CaseFileDate;
                if (index == 3) return InstrumentType;
                if (index == 4) return SettingResults;
                if (index == 5) return CourtNumber;
                if (index == 6) return CaseStatus;
                if (index == 7) return DefendantStatus;
                if (index == 8) return BondAmount;
                if (index == 9) return CurrentOffenseCode;
                if (index == 10) return CurrentOffenseLiteral;
                if (index == 11) return CurrentOffenseLevelAndDegree;
                if (index == 12) return ComplaintOffenseCode;
                if (index == 13) return ComplaintOffenseLiteral;
                if (index == 14) return ComplaintOffenseLevelAndDegree;
                if (index == 15) return GrandJuryOffenseCode;
                if (index == 16) return GrandJuryOffenseLiteral;
                if (index == 17) return GrandJuryOffenseLevelAndDegree;
                if (index == 18) return NextAppearanceDate;
                if (index == 19) return DocketType;
                if (index == 20) return NextAppearanceReason;
                if (index == 21) return DefendantName;
                if (index == 22) return DefendantSpn;
                if (index == 23) return DefendantRace;
                if (index == 24) return DefendantSex;
                if (index == 25) return DefendantDateOfBirth;
                if (index == 26) return DefendantStreetNumber;
                if (index == 27) return DefendantStreetName;
                if (index == 28) return DefendantApartmentNumber;
                if (index == 29) return DefendantCity;
                if (index == 30) return DefendantState;
                if (index == 31) return DefendantZip;
                if (index == 32) return AttorneyName;
                if (index == 33) return AttorneySpn;
                if (index == 34) return AttorneyConnectionCode;
                if (index == 35) return AttorneyConnectionLiteral;
                if (index == 36) return ComplainantName;
                if (index == 37) return ComplainantAgency;
                if (index == 38) return OffenseReportNumber;
                if (index == 39) return DispositionDate;
                if (index == 40) return Disposition;
                if (index == 41) return Sentence;
                if (index == 42) return DefCitizenshipStatus;
                if (index == 43) return BondException;
                if (index == 44) return GrandJuryDate;
                if (index == 45) return GrandJuryCourt;
                if (index == 46) return GrandJuryAction;
                if (index == 47) return DefendantPlaceOfBirth;
                return string.Empty;
            }
            set
            {
                if (index == 0) CourtDivisionIndicator = value;
                if (index == 1) CaseNumber = value;
                if (index == 2) CaseFileDate = value;
                if (index == 3) InstrumentType = value;
                if (index == 4) SettingResults = value;
                if (index == 5) CourtNumber = value;
                if (index == 6) CaseStatus = value;
                if (index == 7) DefendantStatus = value;
                if (index == 8) BondAmount = value;
                if (index == 9) CurrentOffenseCode = value;
                if (index == 10) CurrentOffenseLiteral = value;
                if (index == 11) CurrentOffenseLevelAndDegree = value;
                if (index == 12) ComplaintOffenseCode = value;
                if (index == 13) ComplaintOffenseLiteral = value;
                if (index == 14) ComplaintOffenseLevelAndDegree = value;
                if (index == 15) GrandJuryOffenseCode = value;
                if (index == 16) GrandJuryOffenseLiteral = value;
                if (index == 17) GrandJuryOffenseLevelAndDegree = value;
                if (index == 18) NextAppearanceDate = value;
                if (index == 19) DocketType = value;
                if (index == 20) NextAppearanceReason = value;
                if (index == 21) DefendantName = value;
                if (index == 22) DefendantSpn = value;
                if (index == 23) DefendantRace = value;
                if (index == 24) DefendantSex = value;
                if (index == 25) DefendantDateOfBirth = value;
                if (index == 26) DefendantStreetNumber = value;
                if (index == 27) DefendantStreetName = value;
                if (index == 28) DefendantApartmentNumber = value;
                if (index == 29) DefendantCity = value;
                if (index == 30) DefendantState = value;
                if (index == 31) DefendantZip = value;
                if (index == 32) AttorneyName = value;
                if (index == 33) AttorneySpn = value;
                if (index == 34) AttorneyConnectionCode = value;
                if (index == 35) AttorneyConnectionLiteral = value;
                if (index == 36) ComplainantName = value;
                if (index == 37) ComplainantAgency = value;
                if (index == 38) OffenseReportNumber = value;
                if (index == 39) DispositionDate = value;
                if (index == 40) Disposition = value;
                if (index == 41) Sentence = value;
                if (index == 42) DefCitizenshipStatus = value;
                if (index == 43) BondException = value;
                if (index == 44) GrandJuryDate = value;
                if (index == 45) GrandJuryCourt = value;
                if (index == 46) GrandJuryAction = value;
                if (index == 47) DefendantPlaceOfBirth = value;
            }
        }

    }
}

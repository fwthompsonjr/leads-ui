namespace legallead.jdbc.entities
{
    public class HarrisCriminalUploadDto : BaseDto
    {
        public string CourtDivisionIndicator { get; set; } = string.Empty;
        public string CaseNumber { get; set; } = string.Empty;
        public string CaseFileDate { get; set; } = string.Empty;
        public string InstrumentType { get; set; } = string.Empty;
        public string SettingResults { get; set; } = string.Empty;
        public string CourtNumber { get; set; } = string.Empty;
        public string CaseStatus { get; set; } = string.Empty;
        public string DefendantStatus { get; set; } = string.Empty;
        public string BondAmount { get; set; } = string.Empty;
        public string CurrentOffenseCode { get; set; } = string.Empty;
        public string CurrentOffenseLiteral { get; set; } = string.Empty;
        public string CurrentOffenseLevelAndDegree { get; set; } = string.Empty;
        public string ComplaintOffenseCode { get; set; } = string.Empty;
        public string ComplaintOffenseLiteral { get; set; } = string.Empty;
        public string ComplaintOffenseLevelAndDegree { get; set; } = string.Empty;
        public string GrandJuryOffenseCode { get; set; } = string.Empty;
        public string GrandJuryOffenseLiteral { get; set; } = string.Empty;
        public string GrandJuryOffenseLevelAndDegree { get; set; } = string.Empty;
        public string NextAppearanceDate { get; set; } = string.Empty;
        public string DocketType { get; set; } = string.Empty;
        public string NextAppearanceReason { get; set; } = string.Empty;
        public string DefendantName { get; set; } = string.Empty;
        public string DefendantSpn { get; set; } = string.Empty;
        public string DefendantRace { get; set; } = string.Empty;
        public string DefendantSex { get; set; } = string.Empty;
        public string DefendantDateOfBirth { get; set; } = string.Empty;
        public string DefendantStreetNumber { get; set; } = string.Empty;
        public string DefendantStreetName { get; set; } = string.Empty;
        public string DefendantApartmentNumber { get; set; } = string.Empty;
        public string DefendantCity { get; set; } = string.Empty;
        public string DefendantState { get; set; } = string.Empty;
        public string DefendantZip { get; set; } = string.Empty;
        public string AttorneyName { get; set; } = string.Empty;
        public string AttorneySpn { get; set; } = string.Empty;
        public string AttorneyConnectionCode { get; set; } = string.Empty;
        public string AttorneyConnectionLiteral { get; set; } = string.Empty;
        public string ComplainantName { get; set; } = string.Empty;
        public string ComplainantAgency { get; set; } = string.Empty;
        public string OffenseReportNumber { get; set; } = string.Empty;
        public string DispositionDate { get; set; } = string.Empty;
        public string Disposition { get; set; } = string.Empty;
        public string Sentence { get; set; } = string.Empty;
        public string DefCitizenshipStatus { get; set; } = string.Empty;
        public string BondException { get; set; } = string.Empty;
        public string GrandJuryDate { get; set; } = string.Empty;
        public string GrandJuryCourt { get; set; } = string.Empty;
        public string GrandJuryAction { get; set; } = string.Empty;
        public string DefendantPlaceOfBirth { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("CourtDivisionIndicator", Comparison)) return CourtDivisionIndicator;
                if (fieldName.Equals("CaseNumber", Comparison)) return CaseNumber;
                if (fieldName.Equals("CaseFileDate", Comparison)) return CaseFileDate;
                if (fieldName.Equals("InstrumentType", Comparison)) return InstrumentType;
                if (fieldName.Equals("SettingResults", Comparison)) return SettingResults;
                if (fieldName.Equals("CourtNumber", Comparison)) return CourtNumber;
                if (fieldName.Equals("CaseStatus", Comparison)) return CaseStatus;
                if (fieldName.Equals("DefendantStatus", Comparison)) return DefendantStatus;
                if (fieldName.Equals("BondAmount", Comparison)) return BondAmount;
                if (fieldName.Equals("CurrentOffenseCode", Comparison)) return CurrentOffenseCode;
                if (fieldName.Equals("CurrentOffenseLiteral", Comparison)) return CurrentOffenseLiteral;
                if (fieldName.Equals("CurrentOffenseLevelAndDegree", Comparison)) return CurrentOffenseLevelAndDegree;
                if (fieldName.Equals("ComplaintOffenseCode", Comparison)) return ComplaintOffenseCode;
                if (fieldName.Equals("ComplaintOffenseLiteral", Comparison)) return ComplaintOffenseLiteral;
                if (fieldName.Equals("ComplaintOffenseLevelAndDegree", Comparison)) return ComplaintOffenseLevelAndDegree;
                if (fieldName.Equals("GrandJuryOffenseCode", Comparison)) return GrandJuryOffenseCode;
                if (fieldName.Equals("GrandJuryOffenseLiteral", Comparison)) return GrandJuryOffenseLiteral;
                if (fieldName.Equals("GrandJuryOffenseLevelAndDegree", Comparison)) return GrandJuryOffenseLevelAndDegree;
                if (fieldName.Equals("NextAppearanceDate", Comparison)) return NextAppearanceDate;
                if (fieldName.Equals("DocketType", Comparison)) return DocketType;
                if (fieldName.Equals("NextAppearanceReason", Comparison)) return NextAppearanceReason;
                if (fieldName.Equals("DefendantName", Comparison)) return DefendantName;
                if (fieldName.Equals("DefendantSpn", Comparison)) return DefendantSpn;
                if (fieldName.Equals("DefendantRace", Comparison)) return DefendantRace;
                if (fieldName.Equals("DefendantSex", Comparison)) return DefendantSex;
                if (fieldName.Equals("DefendantDateOfBirth", Comparison)) return DefendantDateOfBirth;
                if (fieldName.Equals("DefendantStreetNumber", Comparison)) return DefendantStreetNumber;
                if (fieldName.Equals("DefendantStreetName", Comparison)) return DefendantStreetName;
                if (fieldName.Equals("DefendantApartmentNumber", Comparison)) return DefendantApartmentNumber;
                if (fieldName.Equals("DefendantCity", Comparison)) return DefendantCity;
                if (fieldName.Equals("DefendantState", Comparison)) return DefendantState;
                if (fieldName.Equals("DefendantZip", Comparison)) return DefendantZip;
                if (fieldName.Equals("AttorneyName", Comparison)) return AttorneyName;
                if (fieldName.Equals("AttorneySpn", Comparison)) return AttorneySpn;
                if (fieldName.Equals("AttorneyConnectionCode", Comparison)) return AttorneyConnectionCode;
                if (fieldName.Equals("AttorneyConnectionLiteral", Comparison)) return AttorneyConnectionLiteral;
                if (fieldName.Equals("ComplainantName", Comparison)) return ComplainantName;
                if (fieldName.Equals("ComplainantAgency", Comparison)) return ComplainantAgency;
                if (fieldName.Equals("OffenseReportNumber", Comparison)) return OffenseReportNumber;
                if (fieldName.Equals("DispositionDate", Comparison)) return DispositionDate;
                if (fieldName.Equals("Disposition", Comparison)) return Disposition;
                if (fieldName.Equals("Sentence", Comparison)) return Sentence;
                if (fieldName.Equals("DefCitizenshipStatus", Comparison)) return DefCitizenshipStatus;
                if (fieldName.Equals("BondException", Comparison)) return BondException;
                if (fieldName.Equals("GrandJuryDate", Comparison)) return GrandJuryDate;
                if (fieldName.Equals("GrandJuryCourt", Comparison)) return GrandJuryCourt;
                if (fieldName.Equals("GrandJuryAction", Comparison)) return GrandJuryAction;
                if (fieldName.Equals("DefendantPlaceOfBirth", Comparison)) return DefendantPlaceOfBirth;
                return CreateDate;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison)) { Id = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CourtDivisionIndicator", Comparison)) { CourtDivisionIndicator = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CaseNumber", Comparison)) { CaseNumber = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CaseFileDate", Comparison)) { CaseFileDate = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("InstrumentType", Comparison)) { InstrumentType = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("SettingResults", Comparison)) { SettingResults = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CourtNumber", Comparison)) { CourtNumber = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CaseStatus", Comparison)) { CaseStatus = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantStatus", Comparison)) { DefendantStatus = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("BondAmount", Comparison)) { BondAmount = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CurrentOffenseCode", Comparison)) { CurrentOffenseCode = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CurrentOffenseLiteral", Comparison)) { CurrentOffenseLiteral = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CurrentOffenseLevelAndDegree", Comparison)) { CurrentOffenseLevelAndDegree = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("ComplaintOffenseCode", Comparison)) { ComplaintOffenseCode = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("ComplaintOffenseLiteral", Comparison)) { ComplaintOffenseLiteral = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("ComplaintOffenseLevelAndDegree", Comparison)) { ComplaintOffenseLevelAndDegree = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("GrandJuryOffenseCode", Comparison)) { GrandJuryOffenseCode = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("GrandJuryOffenseLiteral", Comparison)) { GrandJuryOffenseLiteral = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("GrandJuryOffenseLevelAndDegree", Comparison)) { GrandJuryOffenseLevelAndDegree = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("NextAppearanceDate", Comparison)) { NextAppearanceDate = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DocketType", Comparison)) { DocketType = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("NextAppearanceReason", Comparison)) { NextAppearanceReason = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantName", Comparison)) { DefendantName = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantSpn", Comparison)) { DefendantSpn = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantRace", Comparison)) { DefendantRace = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantSex", Comparison)) { DefendantSex = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantDateOfBirth", Comparison)) { DefendantDateOfBirth = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantStreetNumber", Comparison)) { DefendantStreetNumber = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantStreetName", Comparison)) { DefendantStreetName = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantApartmentNumber", Comparison)) { DefendantApartmentNumber = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantCity", Comparison)) { DefendantCity = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantState", Comparison)) { DefendantState = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantZip", Comparison)) { DefendantZip = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("AttorneyName", Comparison)) { AttorneyName = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("AttorneySpn", Comparison)) { AttorneySpn = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("AttorneyConnectionCode", Comparison)) { AttorneyConnectionCode = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("AttorneyConnectionLiteral", Comparison)) { AttorneyConnectionLiteral = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("ComplainantName", Comparison)) { ComplainantName = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("ComplainantAgency", Comparison)) { ComplainantAgency = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("OffenseReportNumber", Comparison)) { OffenseReportNumber = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DispositionDate", Comparison)) { DispositionDate = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Disposition", Comparison)) { Disposition = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Sentence", Comparison)) { Sentence = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefCitizenshipStatus", Comparison)) { DefCitizenshipStatus = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("BondException", Comparison)) { BondException = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("GrandJuryDate", Comparison)) { GrandJuryDate = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("GrandJuryCourt", Comparison)) { GrandJuryCourt = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("GrandJuryAction", Comparison)) { GrandJuryAction = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DefendantPlaceOfBirth", Comparison)) { DefendantPlaceOfBirth = ChangeType<string>(value) ?? string.Empty; return; }
                CreateDate = ChangeType<DateTime?>(value);
            }
        }

    }
}

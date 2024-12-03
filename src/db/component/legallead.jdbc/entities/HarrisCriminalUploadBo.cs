using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.entities
{
    public class HarrisCriminalUploadBo
    {
        public string Id { get; set; } = string.Empty;
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
    }
}

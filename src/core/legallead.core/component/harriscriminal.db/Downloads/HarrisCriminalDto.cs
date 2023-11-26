using System.Globalization;

namespace legallead.harriscriminal.db.Downloads
{
    public class HarrisCriminalDto
    {
        private static readonly List<string> aliasNames = ("index," +
            "rundate,cdi,cas,fda,ins,cad,crt,cst,dst," +
            "bam,curr_off,curr_off_lit,curr_l_d," +
            "nda,cnc,rea,def_nam,def_spn,def_rac," +
            "def_sex,def_dob,def_stnum,def_stnam," +
            "def_cty,def_st,def_zip," +
            "aty_nam,aty_spn,aty_coc,aty_coc_lit," +
            "def_birthplace,def_uscitizen").Split(',').ToList();

        private static readonly List<string> fieldNames = ("Index," +
            "DateDatasetProduced,CourtDivisionIndicator,CaseNumber," +
            "FilingDate,InstrumentType,CaseDisposition,Court," +
            "CaseStatus,DefendantStatus,BondAmount,CurrentOffense," +
            "CurrentOffenseLiteral,CurrentLevelAndDegree,NextAppearanceDate," +
            "DocketCalendarNameCode,CalendarReason," +
            "DefendantName,DefendantSPN,DefendantRace,DefendantSex,DefendantDateOfBirth," +
            "DefendantStreetNumber,DefendantStreetName,DefendantCity,DefendantState,DefendantZip," +
            "AttorneyName,AttorneySPN,AttorneyConnectionCode,AttorneyConnectionLiteral," +
            "DefendantPlaceOfBirth,DefUSCitizenFlag").Split(',').ToList();

        public int Index { get; set; }
        public string DateDatasetProduced { get; set; } = string.Empty;
        public string CourtDivisionIndicator { get; set; } = string.Empty;
        public string CaseNumber { get; set; } = string.Empty;
        public string FilingDate { get; set; } = string.Empty;
        public string InstrumentType { get; set; } = string.Empty;
        public string CaseDisposition { get; set; } = string.Empty;
        public string Court { get; set; } = string.Empty;
        public string CaseStatus { get; set; } = string.Empty;
        public string DefendantStatus { get; set; } = string.Empty;
        public string BondAmount { get; set; } = string.Empty;
        public string CurrentOffense { get; set; } = string.Empty;
        public string CurrentOffenseLiteral { get; set; } = string.Empty;
        public string CurrentLevelAndDegree { get; set; } = string.Empty;
        public string NextAppearanceDate { get; set; } = string.Empty;
        public string DocketCalendarNameCode { get; set; } = string.Empty;
        public string CalendarReason { get; set; } = string.Empty;
        public string DefendantName { get; set; } = string.Empty;
        public string DefendantSPN { get; set; } = string.Empty;
        public string DefendantRace { get; set; } = string.Empty;
        public string DefendantSex { get; set; } = string.Empty;
        public string DefendantDateOfBirth { get; set; } = string.Empty;
        public string DefendantStreetNumber { get; set; } = string.Empty;
        public string DefendantStreetName { get; set; } = string.Empty;
        public string DefendantCity { get; set; } = string.Empty;
        public string DefendantState { get; set; } = string.Empty;
        public string DefendantZip { get; set; } = string.Empty;
        public string AttorneyName { get; set; } = string.Empty;
        public string AttorneySPN { get; set; } = string.Empty;
        public string AttorneyConnectionCode { get; set; } = string.Empty;
        public string AttorneyConnectionLiteral { get; set; } = string.Empty;
        public string DefendantPlaceOfBirth { get; set; } = string.Empty;
        public string DefUSCitizenFlag { get; set; } = string.Empty;

        public static List<string> AliasNames => aliasNames;

        public static List<string> FieldNames => fieldNames;

        public string? this[int index]
        {
            get
            {
                if (index < 0 || index > FieldNames.Count - 1)
                {
                    return null;
                }
                return index switch
                {
                    0 => Index.ToString("D", CultureInfo.InvariantCulture),
                    1 => DateDatasetProduced,
                    2 => CourtDivisionIndicator,
                    3 => CaseNumber,
                    4 => FilingDate,
                    5 => InstrumentType,
                    6 => CaseDisposition,
                    7 => Court,
                    8 => CaseStatus,
                    9 => DefendantStatus,
                    10 => BondAmount,
                    11 => CurrentOffense,
                    12 => CurrentOffenseLiteral,
                    13 => CurrentLevelAndDegree,
                    14 => NextAppearanceDate,
                    15 => DocketCalendarNameCode,
                    16 => CalendarReason,
                    17 => DefendantName,
                    18 => DefendantSPN,
                    19 => DefendantRace,
                    20 => DefendantSex,
                    21 => DefendantDateOfBirth,
                    22 => DefendantStreetNumber,
                    23 => DefendantStreetName,
                    24 => DefendantCity,
                    25 => DefendantState,
                    26 => DefendantZip,
                    27 => AttorneyName,
                    28 => AttorneySPN,
                    29 => AttorneyConnectionCode,
                    30 => AttorneyConnectionLiteral,
                    31 => DefendantPlaceOfBirth,
                    32 => DefUSCitizenFlag,
                    _ => null,
                };
            }
            set
            {
                if (index < 0 || index > FieldNames.Count)
                {
                    return;
                }
                var vl = value ?? string.Empty;
                switch (index)
                {
                    case 0: Index = Convert.ToInt32(value, CultureInfo.InvariantCulture); return;
                    case 1: DateDatasetProduced = vl; return;
                    case 2: CourtDivisionIndicator = vl; return;
                    case 3: CaseNumber = vl; return;
                    case 4: FilingDate = vl; return;
                    case 5: InstrumentType = vl; return;
                    case 6: CaseDisposition = vl; return;
                    case 7: Court = vl; return;
                    case 8: CaseStatus = vl; return;
                    case 9: DefendantStatus = vl; return;
                    case 10: BondAmount = vl; return;
                    case 11: CurrentOffense = vl; return;
                    case 12: CurrentOffenseLiteral = vl; return;
                    case 13: CurrentLevelAndDegree = vl; return;
                    case 14: NextAppearanceDate = vl; return;
                    case 15: DocketCalendarNameCode = vl; return;
                    case 16: CalendarReason = vl; return;
                    case 17: DefendantName = vl; return;
                    case 18: DefendantSPN = vl; return;
                    case 19: DefendantRace = vl; return;
                    case 20: DefendantSex = vl; return;
                    case 21: DefendantDateOfBirth = vl; return;
                    case 22: DefendantStreetNumber = vl; return;
                    case 23: DefendantStreetName = vl; return;
                    case 24: DefendantCity = vl; return;
                    case 25: DefendantState = vl; return;
                    case 26: DefendantZip = vl; return;
                    case 27: AttorneyName = vl; return;
                    case 28: AttorneySPN = vl; return;
                    case 29: AttorneyConnectionCode = vl; return;
                    case 30: AttorneyConnectionLiteral = vl; return;
                    case 31: DefendantPlaceOfBirth = vl; return;
                    case 32: DefUSCitizenFlag = vl; return;

                    default: return;
                }
            }
        }

        public string? this[string fieldName]
        {
            get
            {
                var index =
                    FieldNames
                    .FindIndex(x => x.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                if (index < 0 || index > FieldNames.Count - 1)
                {
                    return null;
                }
                return this[index];
            }
            set
            {
                var index =
                    FieldNames
                    .FindIndex(x => x.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                if (index < 0 || index > FieldNames.Count)
                {
                    return;
                }
                this[index] = value;
            }
        }

        public static List<HarrisCriminalDto> Map(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new();
            }

            if (!File.Exists(fileName))
            {
                return new();
            }

            var result = new List<HarrisCriminalDto>();
            using (var sreader = new StreamReader(fileName))
            {
                var index = 0;
                var line = sreader.ReadLine();
                while (line != null)
                {
                    if (index <= 0)
                    {
                        index++;
                        continue;
                    }
                    line = sreader.ReadLine();
                    if (line != null)
                    {
                        result.Add(Parse(index, line));
                    }
                    index++;
                }
            }
            return result;
        }

        private static HarrisCriminalDto Parse(int index, string line)
        {
            const string delimiter = "\t";
            var record = new HarrisCriminalDto
            {
                Index = index
            };
            var fields = line.Split(delimiter.ToCharArray());
            for (int i = 0; i < fields.Length; i++)
            {
                var data = Clean(fields[i]);
                record[i + 1] = data;
            }
            return record;
        }

        private static string Clean(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            return text.Trim();
        }
    }
}
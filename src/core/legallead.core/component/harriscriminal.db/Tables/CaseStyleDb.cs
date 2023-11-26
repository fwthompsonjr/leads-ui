using Newtonsoft.Json;
using System.Collections.Immutable;

namespace legallead.harriscriminal.db.Tables
{
    public class CaseStyleDb
    {
        public static ImmutableList<string> FieldNames => fieldNames;

        private static readonly ImmutableList<string> fieldNames =
            ImmutableList.Create("CaseNumber", "Style", "FileDate", "Court", "Status", "TypeOfActionOrOffense");

        [JsonProperty("cnbr")]
        public string CaseNumber { get; set; } = string.Empty;

        [JsonProperty("style")]
        public string Style { get; set; } = string.Empty;

        [JsonProperty("fdt")]
        public string FileDate { get; set; } = string.Empty;

        [JsonProperty("court")]
        public string Court { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("toa")]
        public string TypeOfActionOrOffense { get; set; } = string.Empty;

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
                    0 => CaseNumber,
                    1 => Style,
                    2 => FileDate,
                    3 => Court,
                    4 => Status,
                    5 => TypeOfActionOrOffense,
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
                    case 0: CaseNumber = vl; return;
                    case 1: Style = vl; return;
                    case 2: FileDate = vl; return;
                    case 3: Court = vl; return;
                    case 4: Status = vl; return;
                    case 5: TypeOfActionOrOffense = vl; return;

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
    }
}
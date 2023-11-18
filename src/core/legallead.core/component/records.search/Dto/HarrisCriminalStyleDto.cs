using Newtonsoft.Json;
using System.Collections.Immutable;

namespace legallead.records.search.Dto
{
    public class HarrisCriminalStyleDto
    {
        public static ImmutableList<string> FieldNames => fieldNames;
        private static readonly ImmutableList<string> fieldNames =
            ImmutableList.Create("Index", 
                "CaseNumber", 
                "Style", 
                "FileDate", 
                "Court", 
                "Status", 
                "TypeOfActionOrOffense", 
                "Defendant", 
                "Plantiff");

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the case number.
        /// </summary>
        /// <value>
        /// The case number.
        /// </value>
        [JsonProperty("cnbr")]
        public string CaseNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        [JsonProperty("style")]
        public string Style { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file date.
        /// </summary>
        /// <value>
        /// The file date.
        /// </value>
        [JsonProperty("fdt")] public string FileDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the court.
        /// </summary>
        /// <value>
        /// The court.
        /// </value>
        [JsonProperty("court")] public string Court { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")] public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of action or offense.
        /// </summary>
        /// <value>
        /// The type of action or offense.
        /// </value>
        [JsonProperty("toa")] public string TypeOfActionOrOffense { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the defendant.
        /// </summary>
        /// <value>
        /// The defendant.
        /// </value>
        public string Defendant { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the plantiff.
        /// </summary>
        /// <value>
        /// The plantiff.
        /// </value>
        public string Plantiff { get; set; } = string.Empty;

        public string this[int index]
        {
            get
            {
                if (index < 0 || index > FieldNames.Count - 1)
                {
                    return string.Empty;
                }
                return index switch
                {
                    0 => Index.ToString("D", CultureInfo.InvariantCulture),
                    1 => CaseNumber,
                    2 => Style,
                    3 => FileDate,
                    4 => Court,
                    5 => Status,
                    6 => TypeOfActionOrOffense,
                    7 => Defendant,
                    8 => Plantiff,
                    _ => string.Empty,
                };
            }
            set
            {
                if (index < 0 || index > FieldNames.Count)
                {
                    return;
                }
                switch (index)
                {
                    case 0: Index = Convert.ToInt32(value, CultureInfo.InvariantCulture); return;
                    case 1: CaseNumber = value; return;
                    case 2: Style = value; return;
                    case 3: FileDate = value; return;
                    case 4: Court = value; return;
                    case 5: Status = value; return;
                    case 6: TypeOfActionOrOffense = value; return;
                    case 7: Defendant = value; return;
                    case 8: Plantiff = value; return;

                    default: return;
                }
            }
        }

        public string this[string fieldName]
        {
            get
            {
                int index =
                    FieldNames
                    .FindIndex(x => x.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                if (index < 0 || index > FieldNames.Count - 1)
                {
                    return string.Empty;
                }
                return this[index];
            }
            set
            {
                int index =
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
using legallead.records.search.Classes;
using System.Text;

namespace legallead.records.search.Dto
{
#pragma warning disable CA1716 // Identifiers should not match keywords

    public class Option
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
    }

    public class DropDown
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;

        public List<Option> Options { get; set; } = new();
    }

    public class CaseSearchType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
    }

    public class CaseTypeSelectionDto
    {
        public List<DropDown> DropDowns { get; set; } = new();

        public List<CaseSearchType> CaseSearchTypes { get; set; } = new();

        public static CaseTypeSelectionDto GetDto(string fileSuffix)
        {
            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            var fallback = GetFallbackContent(fileSuffix);
            var data = File.Exists(dataFile) ? File.ReadAllText(dataFile) : fallback;
            if (data.Length == 0)
            {
                throw new FileNotFoundException(CommonKeyIndexes.NavigationFileNotFound);
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CaseTypeSelectionDto>(data) ?? new();
        }

        private static string GetFallbackContent(string fileName)
        {
            var sbb = new StringBuilder();
            const char tilde = '~';
            const char qte = '"';
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            if (fileName.Equals("collinCountyCaseType"))
            {
                sbb.AppendLine("{");
                sbb.AppendLine("  ~dropDowns~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 0,");
                sbb.AppendLine("      ~name~: ~criminal courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~criminal case records~,");
                sbb.AppendLine("          ~query~: ~#divOption1 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 1,");
                sbb.AppendLine("          ~name~: ~probate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption2 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 2,");
                sbb.AppendLine("          ~name~: ~magistrate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption3 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 3,");
                sbb.AppendLine("          ~name~: ~civil and family case records~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 4,");
                sbb.AppendLine("          ~name~: ~justice of the peace case records~,");
                sbb.AppendLine("          ~query~: ~#divOption5 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 1,");
                sbb.AppendLine("      ~name~: ~probate courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~probate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption2 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 2,");
                sbb.AppendLine("      ~name~: ~magistrate courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~magistrate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption3 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 3,");
                sbb.AppendLine("      ~name~: ~civil and family courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~civil and family case records~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 4,");
                sbb.AppendLine("      ~name~: ~justice courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~justice of the peace case records~,");
                sbb.AppendLine("          ~query~: ~#divOption5 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            sbb.Replace(tilde, qte);
            return sbb.ToString().Trim();
        }
    }

    public static class DropDownOptionExtensions
    {
        public static List<DropDown>? ToDropDown(this List<Option> options)
        {
            if (options == null)
            {
                return null;
            }

            List<DropDown> result = new();
            options.ForEach(o =>
            {
                result.Add(new DropDown { Id = o.Id, Name = o.Name, Query = o.Query });
            });
            return result;
        }
    }
}
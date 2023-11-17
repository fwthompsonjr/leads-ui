using legallead.records.search.Classes;

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
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(CommonKeyIndexes.NavigationFileNotFound);
            }
            string data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CaseTypeSelectionDto>(data) ?? new();
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
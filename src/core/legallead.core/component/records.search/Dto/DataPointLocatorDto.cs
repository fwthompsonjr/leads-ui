using legallead.records.search.Classes;

namespace legallead.records.search.Dto
{
    public class DataPoint
    {
        public string Name { get; set; } = string.Empty;
        public string Xpath { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
    }

    public class DataPointLocatorDto
    {
        public IList<DataPoint> DataPoints { get; set; } = new List<DataPoint>();

        public static DataPointLocatorDto GetDto(string fileSuffix)
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
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataPointLocatorDto>(data) ?? new();
        }

        public static DataPointLocatorDto Load(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataPointLocatorDto>(data) ?? new();
        }
    }
}
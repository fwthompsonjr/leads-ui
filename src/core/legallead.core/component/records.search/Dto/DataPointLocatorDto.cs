using legallead.records.search.Classes;

namespace legallead.records.search.Dto
{
    public class DataPoint
    {
        public string Name { get; set; }
        public string Xpath { get; set; }
        public string Result { get; set; }
    }

    public class DataPointLocatorDto
    {
        public IList<DataPoint> DataPoints { get; set; }

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
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataPointLocatorDto>(data);
        }

        public static DataPointLocatorDto Load(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataPointLocatorDto>(data);
        }
    }
}
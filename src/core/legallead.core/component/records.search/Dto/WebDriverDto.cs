using legallead.records.search.Classes;

namespace legallead.records.search.Dto
{
    using JConn = Newtonsoft.Json.JsonConvert;

    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }

    public class WebDrivers
    {
        public int SelectedIndex { get; set; }
        public IList<Driver> Drivers { get; set; } = new List<Driver>();
    }

    public class WebDriverDto
    {
        public WebDrivers WebDrivers { get; set; } = new WebDrivers();
    }

    public static class WebDriverDtoExtensions
    {
        private static string? _dtoContent;
        private static string DtoContent => _dtoContent ??= Get();

        private static string Get()
        {
            string dataFile = Properties.Resources.xml_webDrivers_json;
            return dataFile;
        }

        public static WebDriverDto? Get(this WebDriverDto dto)
        {
            if (dto == null)
            {
                return dto;
            }

            string content = DtoContent;
            return JConn.DeserializeObject<WebDriverDto>(content);
        }
        internal static readonly string SearchSettingFileNotFound
            = CommonKeyIndexes.SearchSettingFileNotFound;
    }
}
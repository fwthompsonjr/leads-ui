using legallead.jdbc.attr;

namespace legallead.jdbc.models
{
    public class DbCountyFileModel
    {
        public string Id { get; set; } = string.Empty;
        [CountyFileType]
        public string FileType { get; set; } = string.Empty;
        [CountyFileStatus]
        public string FileStatus { get; set; } = string.Empty;
        public string FileContent { get; set; } = string.Empty;
    }
}

using legallead.jdbc.entities;

namespace legallead.jdbc.models
{
    public class DbUploadRequest
    {
        public string Id { get; set; } = string.Empty;
        public List<DbSearchHistoryResultBo> Contents { get; set; } = [];
    }
}

namespace legallead.permissions.api.Models
{
    public class ReleaseAssetBo
    {
        public long ReleaseId { get; set; }
        public string Html { get; set; } = string.Empty;
        public List<ReleaseAssetDto> Assets { get; set; } = [];
        public bool IsAssetChecked { get; set; }
    }
}
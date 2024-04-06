namespace legallead.installer.Models
{
    public class LocalAppModel
    {
        public string Name { get; set; } = string.Empty;
        public DateTime PublishDate { get; internal set; }
        public List<LocalVersionModel> Versions { get; set; } = [];
    }
}
namespace legallead.records.search.Models
{
    public class WebFetchResult
    {
        public int WebsiteId { get; set; }
        public List<PersonAddress> PeopleList { get; set; } = new();
        public string CaseList { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
    }
}
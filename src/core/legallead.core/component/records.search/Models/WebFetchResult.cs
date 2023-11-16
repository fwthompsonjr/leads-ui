namespace legallead.records.search.Models
{
    public class WebFetchResult
    {
        public int WebsiteId { get; set; }
        public List<PersonAddress> PeopleList { get; set; }
        public string CaseList { get; set; }
        public string Result { get; set; }
    }
}
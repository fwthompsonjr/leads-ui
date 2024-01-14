
namespace legallead.models.Search
{
    public class SearchNavigationParameter
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<SearchNavigationKey> Keys { get; set; } = new();

        public List<SearchNavInstruction> Instructions { get; set; } = new();

        public List<SearchNavInstruction> CaseInstructions { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

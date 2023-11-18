namespace legallead.records.search.Dto
{
    public class Locator
    {
        public string Find { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
    }

    public class NavigationStep
    {
        public string ActionName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Locator Locator { get; set; } = new();
        public string ExpectedValue { get; set; } = string.Empty;
        public int Wait { get; set; }
    }

    public class NavigationInstructionDto
    {
        public List<NavigationStep> Steps { get; set; } = new();
    }

    public class CourtDropDown
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class TarrantCourtDropDownDto
    {
        public string Name { get; set; } = string.Empty;
        public NavigationStep Step { get; set; } = new();
        public List<CourtDropDown> CourtMap { get; set; } = new();
    }
}
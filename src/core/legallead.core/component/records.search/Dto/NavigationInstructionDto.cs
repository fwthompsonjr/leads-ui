namespace legallead.records.search.Dto
{
    public class Locator
    {
        public string Find { get; set; }
        public string Query { get; set; }
    }

    public class NavigationStep
    {
        public string ActionName { get; set; }
        public string DisplayName { get; set; }
        public Locator Locator { get; set; }
        public string ExpectedValue { get; set; }
        public int Wait { get; set; }
    }

    public class NavigationInstructionDto
    {
        public List<NavigationStep> Steps { get; set; }
    }

    public class CourtDropDown
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class TarrantCourtDropDownDto
    {
        public string Name { get; set; }
        public NavigationStep Step { get; set; }
        public List<CourtDropDown> CourtMap { get; set; }
    }
}
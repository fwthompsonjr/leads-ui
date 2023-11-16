namespace legallead.records.search.Parsing
{
    public class ParseCaseDataByVsStrategy : ParseCaseDataByVersusStrategy
    {
        private const string _searchKeyWord = @"vs";

        public override string SearchFor => _searchKeyWord;
    }
}
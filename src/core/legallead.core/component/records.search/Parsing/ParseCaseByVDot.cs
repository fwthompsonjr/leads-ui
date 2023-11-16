namespace legallead.records.search.Parsing
{
    public class ParseCaseByVDot : ParseCaseDataByVersusStrategy
    {
        private const string _searchKeyWord = @" v. ";

        public override string SearchFor => _searchKeyWord;
    }
}
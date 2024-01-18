namespace legallead.records.search.Classes
{
    public class DentonTableReadHeader
    {
        private const string RowCountPrefix = "Record Count:";
        public string? RowCount { get; set; }
        public string? SearchBy { get; set; }
        public int RowsAffected
        {
            get
            {
                if (string.IsNullOrEmpty(RowCount)) return 0;
                if (RowCount.IndexOf(RowCountPrefix) < 0) return 0;
                var index = RowCount.Split(':')[1].Trim();
                if (int.TryParse(index, out var rowCount)) return rowCount;
                return 0;
            }
        }
    }
}

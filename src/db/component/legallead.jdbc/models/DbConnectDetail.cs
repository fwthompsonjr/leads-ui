namespace legallead.jdbc.models
{
    internal class DbConnectDetail
    {
        public string Local { get; set; } = string.Empty;
        public string Test { get; set; } = string.Empty;
        public string Production { get; set; } = string.Empty;
        public string? this[string field]
        {
            get
            {
                const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                var fieldName = FieldList.Find(x => x.Equals(field, comparison)) ?? string.Empty;
                if (fieldName.Equals("Local", comparison)) return Local;
                if (fieldName.Equals("Test", comparison)) return Test;
                if (fieldName.Equals("Production", comparison)) return Production;
                return null;
            }
        }

        private static readonly List<string> FieldList = new()
        {
            "Local",
            "Test",
            "Production"
        };
    }
}

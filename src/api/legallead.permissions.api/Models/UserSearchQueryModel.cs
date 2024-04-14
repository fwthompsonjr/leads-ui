namespace legallead.permissions.api.Models
{
    public class UserSearchQueryModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? UserId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? EstimatedRowCount { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? SearchProgress { get; set; }

        public string? StateCode { get; set; }

        public string? CountyName { get; set; }

        public string this[int index]
        {
            get
            {
                const string dfmt1 = "Mmm d, YYYY h:mm tt";
                const string dfmt2 = "M/d/YYYY";
                const string dash = " - ";
                if (index < 0 || index > 6) return string.Empty;
                if (index == 0) return Id ?? string.Empty;
                if (index == 1) return CreateDate.HasValue ? CreateDate.Value.ToString(dfmt1) : dash;
                if (index == 2) return StateCode ?? dash;
                if (index == 3) return CountyName ?? dash;
                if (index == 4) return StartDate.HasValue ? StartDate.Value.ToString(dfmt2) : dash;
                if (index == 5) return EndDate.HasValue ? EndDate.Value.ToString(dfmt2) : dash;
                return SearchProgress ?? dash;
            }
        }

    }
}

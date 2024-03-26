namespace legallead.jdbc.entities
{
    public class UserLockStatusDto : BaseDto
    {
        public string? UserId { get; set; }
        public bool? IsLocked { get; set; }
        public int? FailedAttemptCount { get; set; }
        public DateTime? LastFailedAttemptDt { get; set; }
        public DateTime? FailedAttemptResetDt { get; set; }
        public int? MaxFailedAttempts { get; set; }
        public bool? CanResetAccount { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("IsLocked", Comparison)) return IsLocked;
                if (fieldName.Equals("FailedAttemptCount", Comparison)) return FailedAttemptCount;
                if (fieldName.Equals("LastFailedAttemptDt", Comparison)) return LastFailedAttemptDt;
                if (fieldName.Equals("FailedAttemptResetDt", Comparison)) return FailedAttemptResetDt;
                if (fieldName.Equals("MaxFailedAttempts", Comparison)) return MaxFailedAttempts;
                if (fieldName.Equals("CanResetAccount", Comparison)) return CanResetAccount;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison))
                {
                    Id = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("UserId", Comparison)) { UserId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("IsLocked", Comparison)) { IsLocked = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("FailedAttemptCount", Comparison)) { FailedAttemptCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("LastFailedAttemptDt", Comparison)) { LastFailedAttemptDt = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("FailedAttemptResetDt", Comparison)) { FailedAttemptResetDt = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("MaxFailedAttempts", Comparison)) { MaxFailedAttempts = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CanResetAccount", Comparison)) { CanResetAccount = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}
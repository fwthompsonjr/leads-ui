namespace legallead.jdbc.entities
{
    public class UserLockStatusBo
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public bool? IsLocked { get; set; }
        public int? FailedAttemptCount { get; set; }
        public DateTime? LastFailedAttemptDt { get; set; }
        public DateTime? FailedAttemptResetDt { get; set; }
        public int? MaxFailedAttempts { get; set; }
        public bool? CanResetAccount { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}

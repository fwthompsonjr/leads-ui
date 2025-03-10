namespace legallead.jdbc.models
{
    public class UserManagementRequest
    {
        public string RequestId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}

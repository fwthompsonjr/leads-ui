namespace legallead.permissions.api.Model
{
    public class UserSearchBeginResponse
    {
        public string RequestId { get; set; } = string.Empty;
        public UserSearchRequest Request { get; set; } = new();
    }
}

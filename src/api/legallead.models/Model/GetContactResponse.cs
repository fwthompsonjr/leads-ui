namespace legallead.permissions.api.Model
{
    public class GetContactResponse
    {
        public bool IsOK { get; set; }
        public string ResponseType { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
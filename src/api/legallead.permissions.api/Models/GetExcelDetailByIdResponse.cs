namespace legallead.permissions.api.Models
{
    public class GetExcelDetailByIdResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}

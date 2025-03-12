namespace legallead.permissions.api.Models
{
    public class UserManagementOperationResponse(UserManagementOperationRequest request)
    {
        public string MethodName { get; set; } = request.MethodName;
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

using legallead.permissions.api.Attr;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{

    public class UserManagementOperationRequest
    {
        [ValidUserManagementMethod(ErrorMessage = "Invalid method name. Please use one of the predefined method names.")]
        public string MethodName { get; set; } = string.Empty;

        [NullOrEmpty(ErrorMessage = "RequestId must be null or an empty string.")]
        public string RequestId { get; set; } = string.Empty;

        [ValidGuid(ErrorMessage = "UserId must be a valid GUID.")]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(2000, ErrorMessage = "Payload cannot exceed 2000 characters.")]
        [ValidJson(ErrorMessage = "Payload must be a valid JSON object.")]
        public string Payload { get; set; } = string.Empty;
    }
}

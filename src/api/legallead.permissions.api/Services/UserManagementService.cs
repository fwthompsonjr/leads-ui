using legallead.jdbc.interfaces;
using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Models;
using Newtonsoft.Json;

namespace legallead.permissions.api.Services
{
    public class UserManagementService(IUserMangementRepository repo) : IUserManagementService
    {
        private readonly IUserMangementRepository db = repo;
        public async Task<UserManagementOperationResponse> ExecuteAsync(UserManagementOperationRequest request)
        {
            var resp = new UserManagementOperationResponse(request);
            if (!IsValid(request)) return resp;
            UserManagementMethod method = GetMethod(request);
            UserManagementRequest userManagementRequest = new()
            {
                RequestId = request.RequestId,
                UserId = request.UserId,
                Payload = request.Payload,
            };
            var data = method switch
            {
                UserManagementMethod.GetAccounts => GetData(await db.GetAccountsAsync(userManagementRequest)),
                UserManagementMethod.GetCounty => GetData(await db.GetCountyAsync(userManagementRequest)),
                UserManagementMethod.GetInvoice => GetData(await db.GetInvoiceAsync(userManagementRequest)),
                UserManagementMethod.GetPricing => GetData(await db.GetPricingAsync(userManagementRequest)),
                UserManagementMethod.GetProfile => GetData(await db.GetProfileAsync(userManagementRequest)),
                UserManagementMethod.GetSearch => GetData(await db.GetSearchAsync(userManagementRequest)),
                UserManagementMethod.UpdateProfile => GetData(await db.UpdateProfileAsync(userManagementRequest)),
                UserManagementMethod.UpdateUsageLimit => GetData(await db.UpdateUsageLimitAsync(userManagementRequest)),
                _ => "Unknown method"
            };
            if (string.IsNullOrEmpty(data)) return resp;
            resp.IsSuccess = true;
            resp.Message = data;
            return resp;
        }

        private static bool IsValid(UserManagementOperationRequest request)
        {
            if (string.IsNullOrEmpty(request.MethodName)) return false;
            if (!Enum.IsDefined(typeof(UserManagementMethod), request.MethodName)) return false;
            if (string.IsNullOrEmpty(request.RequestId)) return false;
            if (!Guid.TryParse(request.RequestId, out var _)) return false;
            return true;
        }

        private static UserManagementMethod GetMethod(UserManagementOperationRequest request)
        {
            return Enum.Parse<UserManagementMethod>(request.MethodName);
        }

        private static string? GetData(object? data)
        {
            if (data == null) return null;
            if (data is bool isUpdate) return isUpdate ? "true" : "false";
            return JsonConvert.SerializeObject(data);
        }
    }
}

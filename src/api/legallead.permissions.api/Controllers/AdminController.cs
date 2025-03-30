using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("/admin")]
    [ApiController]
    public class AdminController(
    ILeadAuthenicationService lead,
    IUserManagementService db) : ControllerBase
    {
        private readonly ILeadAuthenicationService _leadService = lead;
        private readonly IUserManagementService _dataService = db;

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteAsync(UserManagementOperationRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                model.RequestId = user.Id;
                var response = await _dataService.ExecuteAsync(model);
                var countyList = GetListFromResponse(response);
                if (countyList != null)
                {
                    response.Message = countyList.ToJsonString();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        private static object? GetListFromResponse(UserManagementOperationResponse response)
        {
            if (response.MethodName != "GetCounty") return null;
            if (string.IsNullOrEmpty(response.Message)) return null;
            var data = response.Message.ToInstance<List<LeadUserCountyViewBo>>();
            if (data == null || data.Count == 0) return null;
            return data.Select(d =>
            {
                var view = GetUserView(d);
                return new
                {
                    d.Id,
                    d.RwId,
                    d.CountyId,
                    d.LeadUserId,
                    d.CountyName,
                    view.UserName,
                    view.Credential,
                    d.MonthlyUsage,
                    d.CreateDate,
                };
            });
        }

        private static UserViewBo GetUserView(LeadUserCountyViewBo bo)
        {
            if (string.IsNullOrWhiteSpace(bo.Token)) return new();
            if (string.IsNullOrWhiteSpace(bo.Phrase)) return new();
            if (string.IsNullOrWhiteSpace(bo.Vector)) return new();
            if (!bo.MonthlyUsage.HasValue) return new();
            var decoded = secureSvcs.Decrypt(bo.Token, bo.Phrase, bo.Vector);
            if (string.IsNullOrEmpty(decoded) || !decoded.Contains('|')) return new();
            var items = decoded.Split('|');
            return new UserViewBo
            {
                UserName = items[0],
                Credential = items[^1],
            };
        }
        private class UserViewBo
        {
            public string UserName { get; set; }
            public string Credential { get; set; }
        }
        private static readonly SecureStringService secureSvcs = new();
        private const string UserAccountAccess = "user account access credential";
    }
}

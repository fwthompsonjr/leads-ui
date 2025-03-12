using legallead.permissions.api.Models;
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        private const string UserAccountAccess = "user account access credential";
    }
}

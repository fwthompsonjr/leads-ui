using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly DataProvider _db;
        public PermissionsController(DataProvider db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("set-permission")]
        public async Task<IActionResult> SetPermissionLevel(UserLevelRequest permissionLevel)
        {
            var user = await Request.GetUser(_db);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var validation = permissionLevel.Validate(out var isValid);
            if (!isValid && validation != null)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                return BadRequest(messages);
            }
            var isAdmin = await Request.IsAdminUser(_db);
            if(!isAdmin && permissionLevel.Level.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(permissionLevel);
            }
            var response = await _db.SetPermissionGroup(user, permissionLevel.Level);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }
    }
}

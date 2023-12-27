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
        private readonly ISubscriptionInfrastructure _db;

        public PermissionsController(ISubscriptionInfrastructure db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("add-county-subscription")]
        public async Task<IActionResult> AddCountySubscriptions(CountySubscriptionRequest request)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var validation = request.Validate(out var isValid);
            if (!isValid && validation != null)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                return BadRequest(messages);
            }
            var stateId = _db.FindState(request.State);
            if (stateId == null)
            {
                return BadRequest("State code is invalid.");
            }
            var countyList = _db.FindAllCounties(request.County);
            var countyId = countyList?.Find(l => (l.StateCode ?? "").Equals(stateId.ShortName));
            if (countyId == null)
            {
                return BadRequest("County code is invalid.");
            }
            var response = await _db.AddCountySubscriptions(user, countyId);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        [HttpPost]
        [Route("add-state-subscription")]
        public async Task<IActionResult> AddStateSubscriptions(StateSubscriptionRequest request)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var validation = request.Validate(out var isValid);
            if (!isValid && validation != null)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                return BadRequest(messages);
            }
            var stateId = _db.FindState(request.Name);
            if (stateId == null || string.IsNullOrEmpty(stateId.ShortName))
            {
                return BadRequest("State code is invalid.");
            }
            var response = await _db.AddStateSubscriptions(user, stateId.ShortName);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        [HttpPost]
        [Route("remove-state-subscription")]
        public async Task<IActionResult> RemoveStateSubscriptions(StateSubscriptionRequest request)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var validation = request.Validate(out var isValid);
            if (!isValid && validation != null)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                return BadRequest(messages);
            }
            var stateId = _db.FindState(request.Name);
            if (stateId == null || string.IsNullOrEmpty(stateId.ShortName))
            {
                return BadRequest("State code is invalid.");
            }
            var response = await _db.RemoveStateSubscriptions(user, stateId.ShortName);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        [HttpPost]
        [Route("remove-county-subscription")]
        public async Task<IActionResult> RemoveCountySubscriptions(CountySubscriptionRequest request)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var validation = request.Validate(out var isValid);
            if (!isValid && validation != null)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                return BadRequest(messages);
            }
            var stateId = _db.FindState(request.State);
            if (stateId == null)
            {
                return BadRequest("State code is invalid.");
            }
            var countyList = _db.FindAllCounties(request.County);
            var countyId = countyList?.Find(l => (l.StateCode ?? "").Equals(stateId.ShortName));
            if (countyId == null)
            {
                return BadRequest("County code is invalid.");
            }
            var response = await _db.RemoveCountySubscriptions(user, countyId);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        [HttpPost]
        [Route("set-permission")]
        public async Task<IActionResult> SetPermissionLevel(UserLevelRequest permissionLevel)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var validation = permissionLevel.Validate(out var isValid);
            if (!isValid && validation != null)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                return BadRequest(messages);
            }
            var isAdmin = await _db.IsAdminUser(Request);
            if (!isAdmin && permissionLevel.Level.Equals("admin", StringComparison.OrdinalIgnoreCase))
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
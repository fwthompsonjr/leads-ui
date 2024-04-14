using legallead.models;
using legallead.permissions.api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UsState = legallead.json.db.entity.UsState;
using UsStateCounty = legallead.json.db.entity.UsStateCounty;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private static readonly object locker = new();
        private readonly ISubscriptionInfrastructure _db;
        private readonly ICustomerLockInfrastructure _lockingDb;

        public PermissionsController(ISubscriptionInfrastructure db, ICustomerLockInfrastructure lockingDb)
        {
            _db = db;
            _lockingDb = lockingDb;
            lock (locker)
            {
                UsState.Initialize();
                UsStateCounty.Initialize();
            }
        }

        [HttpPost]
        [Route("set-discount")]
        public async Task<IActionResult> SetDiscount(ChangeDiscountRequest request)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var isAdmin = await _db.IsAdminUser(Request);
            var jsrequest = JsonConvert.SerializeObject(request);
            var session = await _db.GenerateDiscountSession(Request, user, jsrequest, isAdmin, "");
            if (!session.IsPaymentSuccess.GetValueOrDefault())
            {
                return Ok(session);
            }
            return Conflict("Unexpected error during account processing");
        }

        [HttpPost]
        [Route("set-permission")]
        public async Task<IActionResult> SetPermissionLevel(UserLevelRequest permissionLevel)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
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
            var session = await _db.GeneratePermissionSession(Request, user, permissionLevel.Level);
            if (!session.IsPaymentSuccess.GetValueOrDefault())
            {
                return Ok(session);
            }
            return Conflict("Unexpected error during account processing");
        }

    }
}
﻿using legallead.models;
using legallead.permissions.api.Models;
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
        [ServiceFilter(typeof(PermissionChangeRequested))]
        public async Task<IActionResult> SetDiscountAsync(ChangeDiscountRequest request)
        {
            var user = await _db.GetUserAsync(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var isAdmin = await _db.IsAdminUserAsync(Request);
            var jsrequest = JsonConvert.SerializeObject(request);
            var session = await _db.GenerateDiscountSessionAsync(Request, user, jsrequest, isAdmin, "");
            if (!session.IsPaymentSuccess.GetValueOrDefault())
            {
                var serilized = GetChangeResponse("Discount",
                user,
                request,
                session);
                return Ok(serilized);
            }
            return Conflict("Unexpected error during account processing");
        }

        [HttpPost]
        [Route("set-permission")]
        [ServiceFilter(typeof(PermissionChangeRequested))]
        public async Task<IActionResult> SetPermissionLevelAsync(UserLevelRequest permissionLevel)
        {
            var user = await _db.GetUserAsync(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
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
            var isAdmin = await _db.IsAdminUserAsync(Request);
            if (!isAdmin && permissionLevel.Level.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(permissionLevel);
            }
            var session = await _db.GeneratePermissionSessionAsync(Request, user, permissionLevel.Level);
            var serilized = GetChangeResponse("PermissionLevel",
                user,
                permissionLevel,
                session);
            return Ok(serilized);
        }


        private static PermissionChangeModel GetChangeResponse(
            string changeName,
            User user,
            object original,
            LevelRequestBo response)
        {
            var js = JsonConvert.SerializeObject(original);
            var data = new PermissionChangeModel
            {
                Email = user.Email,
                Name = changeName,
                Request = js,
                Dto = response
            };
            return data;
        }

    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class SignonController(
        DataProvider db,
        IJwtManagerRepository jWTManager,
        IRefreshTokenValidator tokenValidator,
        ICustomerLockInfrastructure lockingDb,
        ILoggingInfrastructure logService) : ControllerBase
    {
        private readonly DataProvider _db = db;
        private readonly IJwtManagerRepository _jWTManager = jWTManager;
        private readonly IRefreshTokenValidator _tokenValidator = tokenValidator;
        private readonly ILoggingInfrastructure _logSvc = logService;
        private readonly ICustomerLockInfrastructure _lockingDb = lockingDb;

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> AuthenticateAsync(UserLoginModel usersdata)
        {
            var response = "An error occurred authenticating account.";
            try
            {
                var applicationCheck = Request.Validate(response);
                if (!applicationCheck.Key)
                {
                    await _logSvc.LogWarning("Failed : Validate Application Header. Returning 401 - Unauthorized");
                    return Unauthorized(applicationCheck.Value);
                }
                var model = new UserModel { Password = usersdata.Password, Email = usersdata.UserName, UserName = usersdata.UserName };
                var validUser = await _db.UserDb.IsValidUserAsync(model);
                var user = validUser.Value ?? new();
                var hasIncident = await IsViolationIncidentCreated(validUser);
                if (hasIncident != null) return hasIncident;
                var isLocked = await IsAccountLockedViolation(user.Id, user.Email);
                if (isLocked != null) return isLocked;
                var token = _jWTManager.GenerateToken(user);

                if (token == null)
                {
                    await _logSvc.LogWarning("Failed : Generate Access Token. Returning 401 - Unauthorized");
                    return Unauthorized("Invalid Attempt..");
                }

                var obj = new UserRefreshToken
                {
                    RefreshToken = token.RefreshToken,
                    UserId = user.Id
                };

                await _db.UserTokenDb.Add(obj);
                var permissions = await _db.UserPermissionDb.GetAll(user);
                var emptycount = permissions.Count(c => string.IsNullOrEmpty(c.KeyValue));
                if (emptycount == permissions.Count())
                {
                    await _db.InitializeProfile(user);
                    var initOk = (await _db.InitializePermission(user));
                    await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.AccountRegistrationCompleted);
                    await _db.ProfileHistoryDb.CreateSnapshot(user, jdbc.ProfileChangeTypes.AccountRegistrationCompleted);
                    if (initOk)
                    {
                        await _db.SetPermissionGroup(user, "Guest");
                    }
                }
                return Ok(token);
            }
            catch (Exception ex)
            {
                await _logSvc.LogError(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> Refresh(Tokens token)
        {
            var response = "An error occurred refreshing authentication token.";
            var applicationCheck = Request.Validate(response);
            if (!applicationCheck.Key) { return Unauthorized(applicationCheck.Value); }
            var principal = _jWTManager.GetPrincipalFromExpiredToken(token.AccessToken);
            if (principal == null || principal.Identity == null || string.IsNullOrEmpty(principal.Identity.Name))
            {
                return BadRequest("Invalid access token.");
            }
            var username = principal.Identity.Name ?? string.Empty;
            var user = await _db.UserDb.GetByEmail(username);

            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                return BadRequest("User data is null or empty.");
            }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                await _logSvc.LogWarning("Failed : Account is locked. Returning 403 - Forbidden");
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var found = await _db.UserTokenDb.Find(user.Id, token.RefreshToken);
            var savedRefreshToken = _tokenValidator.Verify(found);
            if (savedRefreshToken == null || !savedRefreshToken.IsActive)
            {
                return Unauthorized("Refresh token is missing or invalid.");
            }

            var newJwtToken = _jWTManager.GenerateRefreshToken(user);
            if (newJwtToken == null)
            {
                return Unauthorized("Failed to generate token.");
            }

            var obj = new UserRefreshToken
            {
                RefreshToken = newJwtToken.RefreshToken,
                UserId = user.Id
            };

            await _db.UserTokenDb.DeleteTokens(user);
            await _db.UserTokenDb.Add(obj);
            return Ok(newJwtToken);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("verify-token")]
        public IActionResult Verify(Tokens token)
        {
            var response = "An error occurred verifying authentication token.";
            var applicationCheck = Request.Validate(response);
            if (!applicationCheck.Key) { return Unauthorized(applicationCheck.Value); }
            var isvalid = _jWTManager.ValidateToken(token.AccessToken);
            if (!isvalid)
            {
                return BadRequest("Invalid access token.");
            }
            return Ok(isvalid);
        }

        [Authorize]
        [HttpPost]
        [Route("change-password")]
        [ServiceFilter(typeof(PasswordChanged))]
        public async Task<IActionResult> ChangePasswordAsync(UserChangePasswordModel usersdata)
        {
            var response = "An error occurred authenticating account.";
            var applicationCheck = Request.Validate(response);
            if (!applicationCheck.Key) { return Unauthorized(applicationCheck.Value); }
            var model = new UserModel { Password = usersdata.OldPassword, Email = usersdata.UserName, UserName = usersdata.UserName };
            var validUser = await _db.UserDb.IsValidUserAsync(model);
            var user = validUser.Value;
            if (!validUser.Key || user == null || string.IsNullOrEmpty(user.Id))
            {
                return Unauthorized("Invalid username or password...");
            }
            var isLocked = await IsAccountLockedViolation(user.Id, user.Email);
            if (isLocked != null) return isLocked;
            User update = MapFromChangePassword(usersdata, model, user);
            update.CreateDate = user.CreateDate.GetValueOrDefault(DateTime.UtcNow);
            await _db.UserDb.Update(update);
            return Ok(usersdata.UserName);
        }
    }
}
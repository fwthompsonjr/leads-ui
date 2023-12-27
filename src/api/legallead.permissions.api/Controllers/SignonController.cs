using legallead.jdbc.entities;
using legallead.jdbc.models;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignonController : ControllerBase
    {
        private readonly DataProvider _db;
        private readonly IJwtManagerRepository _jWTManager;
        private readonly IRefreshTokenValidator _tokenValidator;
        private readonly ILoggingInfrastructure _logSvc;

        public SignonController(
            DataProvider db,
            IJwtManagerRepository jWTManager,
            IRefreshTokenValidator tokenValidator,
            ILoggingInfrastructure logService)
        {
            _db = db;
            _jWTManager = jWTManager;
            _tokenValidator = tokenValidator;
            _logSvc = logService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> AuthenticateAsync(UserLoginModel usersdata)
        {
            var response = "An error occurred authenticating account.";
            await _logSvc.LogInformation("Begin : Authenication process");
            try
            {
                await _logSvc.LogInformation("Begin : Validate Application Header.");
                var applicationCheck = Request.Validate(response);
                await _logSvc.LogInformation("End : Validate Application Header.");
                if (!applicationCheck.Key)
                {
                    await _logSvc.LogWarning("Failed : Validate Application Header. Returning 401 - Unauthorized");
                    return Unauthorized(applicationCheck.Value);
                }
                await _logSvc.LogInformation("Begin : Map Request to User Model.");
                var model = new UserModel { Password = usersdata.Password, Email = usersdata.UserName, UserName = usersdata.UserName };
                await _logSvc.LogInformation("End : Map Request to User Model.");

                await _logSvc.LogInformation("Begin : Validate User Credential.");
                var validUser = await _db.UserDb.IsValidUserAsync(model);
                await _logSvc.LogInformation("End : Validate User Credential.");
                var user = validUser.Value;
                if (!validUser.Key || user == null || string.IsNullOrEmpty(user.Id))
                {
                    await _logSvc.LogWarning("Failed : Validate User Credential. Returning 401 - Unauthorized");
                    return Unauthorized("Invalid username or password...");
                }

                await _logSvc.LogInformation("Begin : Generate Access Token.");
                var token = _jWTManager.GenerateToken(user);
                await _logSvc.LogInformation("End : Generate Access Token.");

                if (token == null)
                {
                    await _logSvc.LogWarning("Failed : Generate Access Token. Returning 401 - Unauthorized");
                    return Unauthorized("Invalid Attempt..");
                }

                await _logSvc.LogInformation("Begin : Map Token to dto.");
                var obj = new UserRefreshToken
                {
                    RefreshToken = token.RefreshToken,
                    UserId = user.Id
                };
                await _logSvc.LogInformation("End : Map Token to dto.");

                await _logSvc.LogInformation("Begin : Save Refresh Token.");
                await _db.UserTokenDb.Add(obj);
                await _logSvc.LogInformation("End : Save Refresh Token.");
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
            User update = MapFromChangePassword(usersdata, model, user);
            await _db.UserDb.Update(update);
            return Ok(usersdata.UserName);
        }

        private static User MapFromChangePassword(UserChangePasswordModel usersdata, UserModel model, User? user)
        {
            model.Password = usersdata.NewPassword;
            if (user != null)
            {
                model.Email = user.Email;
                model.UserName = user.UserName;
            }
            var update = UserModel.ToUser(model);
            if (!string.IsNullOrEmpty(user?.Id)) update.Id = user.Id;
            return update;
        }
    }
}
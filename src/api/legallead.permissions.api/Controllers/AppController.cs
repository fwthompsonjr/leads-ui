using legallead.jdbc.entities;
using legallead.json.db;
using legallead.json.db.entity;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("/app")]
    [ApiController]
    public class AppController: ControllerBase
    {
        private readonly IAppAuthenicationService _authenticationService;
        private readonly ICountyAuthorizationService _authorizationService;
        private readonly ILeadAuthenicationService _leadService;

        public AppController(
        IAppAuthenicationService appservice,
        ICountyAuthorizationService service,
        ILeadAuthenicationService lead)
        {
            _authenticationService = appservice;
            _authorizationService = service;
            _leadService = lead;
            UsStateCounty.Initialize();
        }

        [HttpPost("get-county-code")]
        public IActionResult GetCounty(CountyCodeRequest model)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (!ModelState.IsValid) return BadRequest();
            var response = _authorizationService.Models
                .Find(x => x.Name.Equals(model.Name, comparison) &&
                x.UserId.Equals(model.UserId, comparison)) ?? new();
            return Ok(response);
        }
        [HttpPost("login")]
        public IActionResult Authenicate(AppAuthenicateRequest request)
        {
            var obj = _authenticationService.Authenicate(request.UserName, request.Password);
            if (obj == null) return Unauthorized();
            var response = _authenticationService.FindUser(obj.UserName, obj.Id);
            return Ok(response);
        }

        [HttpPost("account-login")]
        public async Task<IActionResult> AccountAuthenicateAsync(AppAuthenicateRequest request)
        {
            var obj = await _leadService.LoginAsync(request.UserName, request.Password);
            if (string.IsNullOrEmpty(obj)) return Unauthorized();
            var model = obj.ToInstance<LeadUserModel>();
            if (model == null) return Conflict("Failed To Serialize Object");
            var token = LeadTokenService.GenerateToken(UserAccountAccess, model);
            return Ok(new { request.UserName, token });
        }
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccountAsync(RegisterAccountModel model)
        {
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                var response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return BadRequest(response);
            }
            var registration = await _leadService.CreateLoginAsync(model.UserName, model.Password, model.Email);
            if (string.IsNullOrEmpty(registration)) return Conflict();
            return await AccountAuthenicateAsync(new AppAuthenicateRequest
            {
                UserName = model.UserName,
                Password = model.Password
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync(UserChangePasswordModel model)
        {
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                var response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return BadRequest(response);
            }
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var registration = await _leadService.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
            if (!registration) return Conflict();
            return await AccountAuthenicateAsync(new AppAuthenicateRequest
            {
                UserName = model.UserName,
                Password = model.NewPassword
            });
        }

        [HttpPost("set-county-login")]
        public async Task<IActionResult> SetCountyCredentialAsync(UserCountyCredentialModel model)
        {
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                var response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return BadRequest(response);
            }
            var countyId = UsStateCountyList.All.Find(x => (x.ShortName ?? "").Equals(model.CountyName));
            if (countyId == null) return BadRequest($"Invalid county name {model.CountyName}");
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var registration = await _leadService.ChangeCountyCredentialAsync(
                user.Id,
                model.CountyName,
                model.UserName,
                model.Password);
            if (!registration) return Conflict();
            return Ok(countyId);
        }
        private const string UserAccountAccess = "user account access credential";
    }
}

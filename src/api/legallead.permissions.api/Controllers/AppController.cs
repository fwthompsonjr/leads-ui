using legallead.jdbc.interfaces;
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
    public class AppController(
    IAppAuthenicationService appservice,
    ICountyAuthorizationService service,
    ILeadAuthenicationService lead,
    IHarrisLoadRepository harrisdb) : ControllerBase
    {
        private readonly IAppAuthenicationService _authenticationService = appservice;
        private readonly ICountyAuthorizationService _authorizationService = service;
        private readonly ILeadAuthenicationService _leadService = lead;
        private readonly IHarrisLoadRepository _hccDataService = harrisdb;

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
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                var response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return BadRequest(response);
            }
            var countyId = GetCounties.Find(x => (x.Name ?? "").Equals(model.CountyName, oic));
            if (countyId == null) return BadRequest($"Invalid county name {model.CountyName}");
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var registration = await _leadService.ChangeCountyCredentialAsync(
                user.Id,
                model.CountyName,
                model.UserName,
                model.Password);
            if (!registration) return Conflict();
            var updated = await _leadService.GetModelByIdAsync(user.Id);
            if (updated == null) return UnprocessableEntity();
            var token = LeadTokenService.GenerateToken(UserAccountAccess, updated);
            return Ok(new { county = countyId, token });
        }

        [HttpPost("set-county-permission")]
        public async Task<IActionResult> SetCountyPermisionAsync(UserCountyPermissionModel model)
        {
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                var response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return BadRequest(response);
            }
            var counties = model.CountyList;
            var validation = _leadService.VerifyCountyList(counties);
            if (!validation.Key)
            {
                return BadRequest(validation);
            }
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var registration = await _leadService.ChangeCountyPermissionAsync(
                user.Id,
                counties);
            if (!registration) return Conflict();
            var updated = await _leadService.GetModelByIdAsync(user.Id);
            if (updated == null) return UnprocessableEntity();
            var token = LeadTokenService.GenerateToken(UserAccountAccess, updated);
            return Ok(new { counties, token });
        }


        [HttpPost("set-county-usage-limit")]
        public async Task<IActionResult> SetCountyUsageAsync(UserCountyUsageModel model)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                var response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return BadRequest(response);
            }
            var countyId = GetCounties.Find(x => (x.Name ?? "").Equals(model.CountyName, oic));
            if (countyId == null) return BadRequest($"Invalid county name {model.CountyName}");
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var registration = await _leadService.AddCountyUsageAsync(
                user.Id,
                model.CountyName,
                model.MonthlyUsage);
            if (!registration) return Conflict();
            var updated = await _leadService.GetModelByIdAsync(user.Id);
            if (updated == null) return UnprocessableEntity();
            var token = LeadTokenService.GenerateToken(UserAccountAccess, updated);
            return Ok(new { county = countyId, usage = model.MonthlyUsage, token });
        }


        [HttpPost("add-county-usage-record")]
        public async Task<IActionResult> AddCountyUsageRecordAsync(UserCountyUsageModel model)
        {
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                var response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return BadRequest(response);
            }
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var registration = await _leadService.AddCountyUsageIncidentAsync(
                user.Id,
                model.CountyName,
                model.MonthlyUsage,
                model.DateRange ?? "");
            if (!registration) return Conflict();
            var updated = await _leadService.GetModelByIdAsync(user.Id);
            if (updated == null) return UnprocessableEntity();
            var token = LeadTokenService.GenerateToken(UserAccountAccess, updated);
            return Ok(new { county = model.CountyName, usage = model.MonthlyUsage, token });
        }


        [HttpPost("get-usage-list")]
        public async Task<IActionResult> GetUserUsageAsync(UserCountyUsageRequest model)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var registration = await _leadService.GetUsageUserByIdAsync(
                user.Id);
            registration = registration.FindAll(x => x.IncidentMonth == model.CreateDate.Month
                && x.IncidentYear == model.CreateDate.Year);
            return Ok(registration);
        }

        [HttpPost("load-hcc-data")]
        public async Task<IActionResult> LoadHccDataAsync(LoadHccDataRequest model)
        {
            try
            {
                var loader = new HccRecordLoadingService(model.Content, _hccDataService);
                await loader.LoadAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost("find-hcc-data")]
        public async Task<IActionResult> FindHccDataAsync(FindHccDataRequest model)
        {
            try
            {
                var response = await _hccDataService.Find(model.FilingDate);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }


        [HttpPost("count-hcc-data")]
        public async Task<IActionResult> CountHccDataAsync(FindHccDataRequest model)
        {
            try
            {
                var response = await _hccDataService.Count(model.FilingDate);
                return Ok(new { RecordCount = response });
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        private const string UserAccountAccess = "user account access credential";
        private static List<UsStateCounty> GetCounties
        {
            get
            {
                if (txcounties != null) return txcounties;
                UsStateCounty.Initialize();
                txcounties = new(UsStateCountyList.All.FindAll(x => x.StateCode == "TX"));
                return txcounties;
            }
        }
        private static List<UsStateCounty>? txcounties;
    }
}

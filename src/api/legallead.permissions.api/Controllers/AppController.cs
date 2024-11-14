using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("/app")]
    [ApiController]
    public class AppController(
        IAppAuthenicationService appservice,
        ICountyAuthorizationService service,
        ILeadAuthenicationService lead) : ControllerBase
    {
        private readonly IAppAuthenicationService _authenticationService = appservice;
        private readonly ICountyAuthorizationService _authorizationService = service;
        private readonly ILeadAuthenicationService _leadService = lead;

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
        /*
        
        [HttpPost("account-login")]
        public async Task<IActionResult> AccountAuthenicate(AppAuthenicateRequest request)
        {
            var obj = await _leadService.LoginAsync(request.UserName, request.Password);
            if (string.IsNullOrEmpty(obj)) return Unauthorized();
            var response = _authenticationService.FindUser(obj.UserName, obj.Id);
            return Ok(response);
        }
        */
    }
}

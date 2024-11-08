using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("/app")]
    [ApiController]
    public class AppController(ICountyAuthorizationService service) : ControllerBase
    {
        private readonly ICountyAuthorizationService _authorizationService = service;

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

    }
}

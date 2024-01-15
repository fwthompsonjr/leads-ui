using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IUserSearchValidator searchValidator;
        private readonly ISearchInfrastructure infrastructure;

        public SearchController(IUserSearchValidator validator, ISearchInfrastructure infrastructure)
        {
            searchValidator = validator;
            this.infrastructure = infrastructure;
        }

        [HttpPost]
        [Route("search-begin")]
        public async Task<IActionResult> BeginSearch(UserSearchRequest request)
        {
            var user = await infrastructure.GetUser(Request);
            if (user == null) { return Unauthorized(); }
            var isValid = searchValidator.IsValid(request);
            if (!isValid.Key)
            {
                return BadRequest(isValid);
            }
            var result = await infrastructure.Begin(Request, request);
            if (result == null) return Conflict(request);
            if (string.IsNullOrWhiteSpace(result.RequestId)) return UnprocessableEntity(result);
            return Ok(result);
        }
    }
}

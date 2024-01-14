using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var isValid = searchValidator.IsValid(request);
            if (!isValid.Key)
            {
                return BadRequest(isValid);
            }
            var result = await infrastructure.Begin(Request, request);
            if (result == null) return Conflict(request);
            return Ok(result);
        }
    }
}

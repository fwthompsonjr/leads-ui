using legallead.permissions.api.Model;
using legallead.permissions.api.Utility;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly UserSearchValidator searchValidator;
        public SearchController(UserSearchValidator validator)
        {
            searchValidator = validator;
        }

        [HttpPost]
        [Route("search-begin")]
        public IActionResult BeginSearch(UserSearchRequest request)
        {
            var isValid = searchValidator.IsValid(request);
            if (!isValid.Key)
            {
                return BadRequest(isValid);
            }
            return Ok(isValid);
        }
    }
}

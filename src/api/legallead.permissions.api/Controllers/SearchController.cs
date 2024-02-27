using legallead.permissions.api.Interfaces;
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

        public SearchController(IUserSearchValidator validator,
            ISearchInfrastructure infrastructure)
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
        [HttpPost]
        [Route("my-searches")]
        public async Task<IActionResult> MySearches(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var searches = await infrastructure.GetHeader(Request, null);
            return Ok(searches);
        }

        [HttpPost]
        [Route("my-active-searches")]
        public async Task<IActionResult> MyActiveSearches(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var detail = await infrastructure.GetSearchDetails(user.Id);
            return Ok(detail);
        }

        [HttpPost]
        [Route("my-search-preview")]
        public async Task<IActionResult> Preview(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var searches = await infrastructure.GetPreview(Request, guid);
            if (searches == null) return UnprocessableEntity(guid);
            return Ok(searches);
        }


        [HttpPost]
        [Route("my-search-status")]
        public async Task<IActionResult> SearchStatus(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var searches = await infrastructure.GetSearchProgress(guid);
            return Ok(searches);
        }

        [HttpPost]
        [Route("my-purchases")]
        public async Task<IActionResult> MyPurchases(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var searches = await infrastructure.GetPurchases(user.Id);
            return Ok(searches);
        }
    }
}

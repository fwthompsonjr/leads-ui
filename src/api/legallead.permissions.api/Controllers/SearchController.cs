using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController(
        IUserSearchValidator validator,
        ISearchInfrastructure infrastructure,
        ICustomerLockInfrastructure lockingDb) : ControllerBase
    {
        private readonly IUserSearchValidator searchValidator = validator;
        private readonly ISearchInfrastructure infrastructure = infrastructure;
        private readonly ICustomerLockInfrastructure _lockingDb = lockingDb;

        [HttpPost]
        [Route("search-begin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType<UserSearchBeginResponse>(StatusCodes.Status200OK)]
        [ServiceFilter(typeof(BeginSearchRequested))]
        public async Task<IActionResult> BeginSearch(UserSearchRequest request)
        {
            var user = await infrastructure.GetUser(Request);
            if (user == null) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<UserSearchQueryModel>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> MySearches(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetHeader(Request, null);
            return Ok(searches);
        }

        [HttpPost]
        [Route("my-active-searches")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<object>(StatusCodes.Status200OK)]
        public async Task<IActionResult> MyActiveSearches(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var detail = await infrastructure.GetSearchDetails(user.Id);
            return Ok(detail);
        }

        [HttpPost]
        [Route("my-search-preview")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType<IEnumerable<SearchPreviewBo>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Preview(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetPreview(Request, guid);
            if (searches == null) return UnprocessableEntity(guid);
            return Ok(searches);
        }

        [HttpPost]
        [Route("my-restriction-status")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<SearchRestrictionModel>(StatusCodes.Status200OK)]
        public async Task<IActionResult> RestrictionStatus(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var status = await infrastructure.GetRestrictionStatus(Request);
            return Ok(status);
        }

        [HttpPost]
        [Route("my-search-status")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ActiveSearchOverviewBo>(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchStatus(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetSearchProgress(guid);
            return Ok(searches);
        }

        [HttpPost]
        [Route("my-purchases")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<PurchasedSearchBo>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> MyPurchases(ApplicationModel context)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetPurchases(user.Id);
            return Ok(searches);
        }

        [HttpGet]
        [Route("list-my-purchases")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<PurchasedSearchBo>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListMyPurchases([FromQuery] string userName)
        {
            var user = await infrastructure.GetUser(Request);
            var email = user?.Email ?? string.Empty;
            var alias = user?.UserName ?? string.Empty;

            var isvalid = userName.Equals(alias) || userName.Equals(email);
            if (string.IsNullOrEmpty(userName) || !isvalid || user == null) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetPurchases(user.Id);
            if (searches == null || !searches.Any()) { return Ok(Array.Empty<PurchasedSearchBo>()); }
            var list = searches.ToList();
            list.Sort((a, b) => b.PurchaseDate.GetValueOrDefault().CompareTo(a.PurchaseDate.GetValueOrDefault()));
            return Ok(list);
        }
    }
}

using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        public async Task<IActionResult> BeginSearchAsync(UserSearchRequest request)
        {
            var user = await infrastructure.GetUserAsync(Request);
            if (user == null) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var isValid = searchValidator.IsValid(request);
            if (!isValid.Key)
            {
                return BadRequest(isValid);
            }
            var result = await infrastructure.BeginAsync(Request, request);
            if (result == null) return Conflict(request);
            if (string.IsNullOrWhiteSpace(result.RequestId)) return UnprocessableEntity(result);
            return Ok(result);
        }

        [HttpPost]
        [Route("my-searches")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<UserSearchQueryModel>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> MySearchesAsync(ApplicationModel context)
        {
            var user = await infrastructure.GetUserAsync(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            try
            {

                var searches = await infrastructure.GetHeaderAsync(Request, null);
                return Ok(searches);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Ok(Array.Empty<UserSearchQueryModel>());
            }
        }

        [HttpPost]
        [Route("my-searches-count")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<UserSearchQueryModel>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> MySearchesCountAsync(ApplicationModel context)
        {
            var response = await MySearchesAsync(context);
            if (response is not OkObjectResult ok) return response;
            var count = new { Count = 0 };
            if (ok.Value is not IEnumerable<UserSearchQueryModel> searches) return Ok(count);
            var actual = new { Count = searches.Count() };
            return Ok(actual);
        }

        [HttpPost]
        [Route("my-active-searches")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<object>(StatusCodes.Status200OK)]
        public async Task<IActionResult> MyActiveSearchesAsync(ApplicationModel context)
        {
            var user = await infrastructure.GetUserAsync(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var detail = await infrastructure.GetSearchDetailsAsync(user.Id);
            return Ok(detail);
        }

        [HttpPost]
        [Route("my-search-preview")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType<IEnumerable<SearchPreviewBo>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> PreviewAsync(ApplicationModel context)
        {
            var user = await infrastructure.GetUserAsync(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetPreviewAsync(Request, guid);
            if (searches == null) return UnprocessableEntity(guid);
            return Ok(searches);
        }

        [HttpPost]
        [Route("my-restriction-status")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<SearchRestrictionModel>(StatusCodes.Status200OK)]
        public async Task<IActionResult> RestrictionStatusAsync(ApplicationModel context)
        {
            var user = await infrastructure.GetUserAsync(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var status = await infrastructure.GetRestrictionStatusAsync(Request);
            return Ok(status);
        }

        [HttpPost]
        [Route("my-search-status")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ActiveSearchOverviewBo>(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchStatusAsync(ApplicationModel context)
        {
            var user = await infrastructure.GetUserAsync(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetSearchProgressAsync(guid);
            return Ok(searches);
        }

        [HttpPost]
        [Route("my-purchases")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<PurchasedSearchBo>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> MyPurchasesAsync(ApplicationModel context)
        {
            var user = await infrastructure.GetUserAsync(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetPurchasesAsync(user.Id);
            return Ok(searches);
        }

        [HttpPost]
        [Route("search-extend-restriction")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<bool?>(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExtendRestrictionAsync(ApplicationModel context)
        {
            var user = await infrastructure.GetUserAsync(Request);
            var guid = context.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.ExtendRestrictionAsync(Request);
            return Ok(searches);
        }

        [HttpGet]
        [Route("list-my-purchases")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<PurchasedSearchBo>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListMyPurchasesAsync([FromQuery] string userName)
        {
            var user = await infrastructure.GetUserAsync(Request);
            var email = user?.Email ?? string.Empty;
            var alias = user?.UserName ?? string.Empty;

            var isvalid = userName.Equals(alias) || userName.Equals(email);
            if (string.IsNullOrEmpty(userName) || !isvalid || user == null) { return Unauthorized(); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var searches = await infrastructure.GetPurchasesAsync(user.Id);
            if (searches == null || !searches.Any()) { return Ok(Array.Empty<PurchasedSearchBo>()); }
            var list = searches.ToList();
            list.Sort((a, b) => b.PurchaseDate.GetValueOrDefault().CompareTo(a.PurchaseDate.GetValueOrDefault()));
            return Ok(list);
        }
    }
}

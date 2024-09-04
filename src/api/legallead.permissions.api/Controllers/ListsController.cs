using AutoMapper;
using legallead.json.db;
using legallead.json.db.entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ListsController : ControllerBase
    {
        private readonly DataProvider _db;
        private readonly IMapper _mapper;
        private readonly ICustomerLockInfrastructure _lockingDb;

        public ListsController(DataProvider db, ICustomerLockInfrastructure lockingDb)
        {
            _db = db;
            _lockingDb = lockingDb;
            UsState.Initialize();
            UsStateCounty.Initialize();
            _mapper = ModelMapper.Mapper;
        }

        [HttpGet]
        [Route("us-state-list")]
        [AllowAnonymous]
        [ProducesResponseType<IEnumerable<UsState>>(StatusCodes.Status200OK)]
        public IActionResult GetStateDetails()
        {
            var states = UsStatesList.All.Where(x => x.IsActive);
            return Ok(states);
        }

        [HttpGet]
        [Route("us-county-list")]
        [AllowAnonymous]
        [ProducesResponseType<IEnumerable<UsStateCounty>>(StatusCodes.Status200OK)]
        public IActionResult GetCountyDetails()
        {
            var counties = new List<UsStateCounty>();
            var source = UsStateCountyList.All.Where(x => x.IsActive);
            counties.AddRange(source);
            return Ok(counties);
        }

        [HttpGet]
        [Route("permission-groups")]
        [AllowAnonymous]
        [ProducesResponseType<IEnumerable<PermissionGroupModel>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPermissionGroupsAsync()
        {
#if DEBUG
            var isAdmin = await Request.IsAdminUserAsync(_db);
#else
            var user = await Request.GetUser(_db);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var isAdmin = await Request.IsAdminUser(_db);
#endif

            var permissions = (await _db.PermissionGroupDb.GetAll()).Where(p => isAdmin || p.IsVisible.GetValueOrDefault());
            var models = permissions.Select(s => _mapper.Map<PermissionGroupModel>(s)).ToList();
            models.FindAll(m => m.IsActive.GetValueOrDefault());
            models = models.FindAll(m => !m.Name.Contains('.'));
            models.Sort((a, b) =>
            {
                return a.GroupId.GetValueOrDefault().CompareTo(b.GroupId.GetValueOrDefault());
            });
            return Ok(models);
        }

        [HttpGet]
        [Route("user-profile")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<UserProfileModel>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            var user = await Request.GetUserAsync(_db);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            await _db.InitializeProfileAsync(user);
            var profiles = await _db.UserProfileVw.GetAll(user);
            var models = profiles.Select(s => _mapper.Map<UserProfileModel>(s)).ToList();
            models.ForEach(m => m.UserName = user.UserName);
            models.Sort((a, b) => (a.KeyName ?? string.Empty).CompareTo(b.KeyName ?? string.Empty));
            return Ok(models);
        }

        [HttpGet]
        [Route("user-permissions")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<IEnumerable<UserProfileModel>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserPermissionsAsync()
        {
            var user = await Request.GetUserAsync(_db);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            await _db.InitializePermissionAsync(user);
            var profiles = await _db.UserPermissionVw.GetAll(user);
            var models = profiles.Select(s => _mapper.Map<UserProfileModel>(s)).ToList();
            models.ForEach(m => m.UserName = user.UserName);
            models.Sort((a, b) => (a.KeyName ?? string.Empty).CompareTo(b.KeyName ?? string.Empty));
            return Ok(models);
        }
    }
}
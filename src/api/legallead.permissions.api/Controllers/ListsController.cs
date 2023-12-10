using AutoMapper;
using legallead.json.db;
using legallead.json.db.entity;
using legallead.permissions.api.Model;
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

        public ListsController(DataProvider db)
        {
            _db = db;
            UsState.Initialize();
            UsStateCounty.Initialize();
            _mapper = ModelMapper.Mapper;
        }

        [HttpGet]
        [Route("us-state-list")]
        [AllowAnonymous]
        public IActionResult GetStateDetails()
        {
            var states = UsStatesList.All.Where(x => x.IsActive);
            return Ok(states);
        }

        [HttpGet]
        [Route("us-county-list")]
        [AllowAnonymous]
        public IActionResult GetCountyDetails()
        {
            var counties = UsStateCountyList.All.Where(x => x.IsActive);
            return Ok(counties);
        }

        [HttpGet]
        [Route("permission-groups")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPermissionGroups()
        {
#if DEBUG
            var isAdmin = await Request.IsAdminUser(_db);
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
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await Request.GetUser(_db);
            if (user == null) { return Unauthorized("Invalid user account."); }
            await _db.InitializeProfile(user);
            var profiles = await _db.UserProfileVw.GetAll(user);
            var models = profiles.Select(s => _mapper.Map<UserProfileModel>(s)).ToList();
            models.ForEach(m => m.UserName = user.UserName);
            models.Sort((a, b) => (a.KeyName ?? string.Empty).CompareTo(b.KeyName ?? string.Empty));
            return Ok(models);
        }

        [HttpGet]
        [Route("user-permissions")]
        public async Task<IActionResult> GetUserPermissions()
        {
            var user = await Request.GetUser(_db);
            if (user == null) { return Unauthorized("Invalid user account."); }
            await _db.InitializePermission(user);
            var profiles = await _db.UserPermissionVw.GetAll(user);
            var models = profiles.Select(s => _mapper.Map<UserProfileModel>(s)).ToList();
            models.ForEach(m => m.UserName = user.UserName);
            models.Sort((a, b) => (a.KeyName ?? string.Empty).CompareTo(b.KeyName ?? string.Empty));
            return Ok(models);
        }
    }
}
using AutoMapper;
using legallead.json.db.entity;
using legallead.json.db.interfaces;
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
        private readonly IJsonDataProvider _jsondb;
        private readonly UsState stateRef = new();
        private readonly UsStateCounty countyRef = new();
        private readonly IMapper _mapper;

        public ListsController(DataProvider db, IJsonDataProvider jsondb)
        {
            _db = db;
            _jsondb = jsondb;
            _mapper = ModelMapper.Mapper;
        }

        [HttpGet]
        [Route("us-state-list")]
        public IActionResult GetStateDetails()
        {
            var states = _jsondb.Where(stateRef, x => x.IsActive);
            return Ok(states);
        }

        [HttpGet]
        [Route("us-county-list")]
        public IActionResult GetCountyDetails()
        {
            var counties = _jsondb.Where(countyRef, x => x.IsActive);
            return Ok(counties);
        }

        [HttpGet]
        [Route("permission-groups")]
        public async Task<IActionResult> GetPermissionGroups()
        {
            var user = await Request.GetUser(_db);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var isAdmin = await Request.IsAdminUser(_db);
            var permissions = (await _db.PermissionGroupDb.GetAll()).Where(p => isAdmin || p.IsVisible.GetValueOrDefault());
            var models = permissions.Select(s => _mapper.Map<PermissionGroupModel>(s)).ToList();
            models.FindAll(m => m.IsActive.GetValueOrDefault());
            models = models.FindAll(m => !m.Name.Contains('.'));
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
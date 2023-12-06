using legallead.json.db.entity;
using legallead.json.db.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly DataProvider _db;
        private readonly IJsonDataProvider _jsondb;
        private readonly UsState stateRef = new();
        private readonly UsStateCounty countyRef = new();
        public SubscriptionsController(DataProvider db, IJsonDataProvider jsondb)
        {
            _db = db;
            _jsondb = jsondb;
        }

        [HttpGet]
        public IActionResult GetStateDetails()
        {
            var states = _jsondb.Where(stateRef, x => x.IsActive);
            return Ok(states);
        }

        [HttpGet]
        public IActionResult GetCountyDetails()
        {
            var counties = _jsondb.Where(countyRef, x => x.IsActive);
            return Ok(counties);
        }
    }
}

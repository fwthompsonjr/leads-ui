using legallead.jdbc.entities;
using legallead.json.db.entity;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private static readonly object locker = new();
        private readonly ISubscriptionInfrastructure _db;

        public PermissionsController(ISubscriptionInfrastructure db)
        {
            _db = db;
            lock (locker)
            {
                UsState.Initialize();
                UsStateCounty.Initialize();
            }
        }

        [HttpPost]
        [Route("set-discount")]
        public async Task<IActionResult> SetDiscount(ChangeDiscountRequest request)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var statelist = ModelMapper.Mapper.Map<List<KeyValuePair<bool, UsState>>>(request);

            List<IActionResult> list = ProcessStateDiscounts(user, statelist);
            var failed = list.Find(a => a is not OkObjectResult);
            if (failed != null) return failed;

            var countylist = ModelMapper.Mapper.Map<List<KeyValuePair<bool, UsStateCounty>>>(request);
            list.AddRange(ProcessCountyDiscounts(user, countylist));
            var success = list.FindAll(a => a is OkObjectResult);
            failed = list.Find(a => a is not OkObjectResult);

            if (failed != null) return failed;
            if (success != null && success.Any()) { return Consolidate(success); }
            return Conflict("Unexpected error during account processing");
        }

        [HttpPost]
        [Route("set-permission")]
        public async Task<IActionResult> SetPermissionLevel(UserLevelRequest permissionLevel)
        {
            var user = await _db.GetUser(Request);
            if (user == null) { return Unauthorized("Invalid user account."); }
            var validation = permissionLevel.Validate(out var isValid);
            if (!isValid && validation != null)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                return BadRequest(messages);
            }
            var isAdmin = await _db.IsAdminUser(Request);
            if (!isAdmin && permissionLevel.Level.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(permissionLevel);
            }
            var response = await _db.SetPermissionGroup(user, permissionLevel.Level);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        protected virtual async Task<IActionResult> AddCountySubscriptions(User user, CountySubscriptionRequest request)
        {
            var stateId = _db.FindState(request.State);
            if (stateId == null)
            {
                return BadRequest("State code is invalid.");
            }
            var countyList = _db.FindAllCounties(request.County);
            var countyId = countyList?.Find(l => (l.StateCode ?? "").Equals(stateId.ShortName));
            if (countyId == null)
            {
                return BadRequest("County code is invalid.");
            }
            var response = await _db.AddCountySubscriptions(user, countyId);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        protected virtual async Task<IActionResult> AddStateSubscriptions(User user, StateSubscriptionRequest request)
        {
            var stateId = _db.FindState(request.Name);
            if (stateId == null || string.IsNullOrEmpty(stateId.ShortName))
            {
                return BadRequest("State code is invalid.");
            }
            var response = await _db.AddStateSubscriptions(user, stateId.ShortName);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        protected virtual async Task<IActionResult> RemoveStateSubscriptions(User user, StateSubscriptionRequest request)
        {
            var stateId = _db.FindState(request.Name);
            if (stateId == null || string.IsNullOrEmpty(stateId.ShortName))
            {
                return BadRequest("State code is invalid.");
            }
            var response = await _db.RemoveStateSubscriptions(user, stateId.ShortName);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        protected virtual async Task<IActionResult> RemoveCountySubscriptions(User user, CountySubscriptionRequest request)
        {
            var stateId = _db.FindState(request.State);
            if (stateId == null)
            {
                return BadRequest("State code is invalid.");
            }
            var countyList = _db.FindAllCounties(request.County);
            var countyId = countyList?.Find(l => (l.StateCode ?? "").Equals(stateId.ShortName));
            if (countyId == null)
            {
                return BadRequest("County code is invalid.");
            }
            var response = await _db.RemoveCountySubscriptions(user, countyId);

            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        private List<IActionResult> ProcessStateDiscounts(User user, List<KeyValuePair<bool, UsState>> statelist)
        {
            List<IActionResult> list = new();
            statelist.ForEach(st =>
            {
                var failed = list.Find(a => a is not OkObjectResult);
                if (failed == null)
                {
                    IActionResult? stateResult;
                    var stateRequest = new StateSubscriptionRequest { Name = st.Value.Name };
                    if (st.Key)
                    {
                        stateResult = AddStateSubscriptions(user, stateRequest).Result;
                    }
                    else
                    {
                        stateResult = RemoveStateSubscriptions(user, stateRequest).Result;
                    }
                    if (stateResult != null) { list.Add(stateResult); }
                }
            });
            return list;
        }

        private List<IActionResult> ProcessCountyDiscounts(User user, List<KeyValuePair<bool, UsStateCounty>> countylist)
        {
            List<IActionResult> list = new();
            countylist.ForEach(c =>
            {
                var failed = list.Find(a => a is not OkObjectResult);
                if (failed == null)
                {
                    IActionResult? countyResult;
                    var county = c.Value;
                    var countyRequest = new CountySubscriptionRequest
                    {
                        County = county.Name,
                        State = county.StateCode
                    };
                    if (c.Key)
                    {
                        countyResult = AddCountySubscriptions(user, countyRequest).Result;
                    }
                    else
                    {
                        countyResult = RemoveCountySubscriptions(user, countyRequest).Result;
                    }
                    if (countyResult != null) { list.Add(countyResult); }
                }
            });
            return list;
        }

        private static OkObjectResult Consolidate(List<IActionResult> results)
        {
            var messages = new List<string>();
            foreach (var result in results)
            {
                if (result is OkObjectResult result1)
                {
                    var msg = Convert.ToString(result1.Value);
                    if (!string.IsNullOrEmpty(msg))
                        messages.Add(msg);
                }
            }
            return new(messages);
        }
    }
}
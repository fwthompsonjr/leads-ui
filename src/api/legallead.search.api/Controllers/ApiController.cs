using legallead.jdbc.interfaces;
using legallead.models.Search;
using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace legallead.search.api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IUserSearchRepository _repo;

        public ApiController(IServiceProvider provider) : this(provider.GetRequiredService<IUserSearchRepository>()) { }
        internal ApiController(IUserSearchRepository repo)
        {
            _repo = repo;
        }
        [HttpPost]
        [Route("begin")]
        public async Task<IActionResult> IndexAsync([FromBody] UserSearchBeginResponse source)
        {
            var uniqueId = source.RequestId;
            if (string.IsNullOrWhiteSpace(uniqueId))
            {
                return BadRequest("Search Index is null or empty.");
            }
            var searchRecord = await _repo.GetTargets(jdbc.SearchTargetTypes.Request, null, uniqueId);
            if (searchRecord == null || !searchRecord.Any() || string.IsNullOrWhiteSpace(searchRecord.First().SearchId))
            {
                return BadRequest("Search Index is invalid or not found.");
            }
            var js = WebMapper.MapFrom<UserSearchRequest, SearchNavigationParameter>(source.Request);
            if (js == null)
            {
                await _repo.Append(jdbc.SearchTargetTypes.Status, uniqueId, "ERROR: Search request format is invalid.");
                return UnprocessableEntity(source.Request);
            }
            js.Name = uniqueId;
            var obj = JsonConvert.SerializeObject(js);
            var mapped = await _repo.Append(jdbc.SearchTargetTypes.Detail, uniqueId, obj);
            if (!mapped.Key)
            {
                await _repo.Append(jdbc.SearchTargetTypes.Status, uniqueId, "ERROR: Search request internal database conflict.");
                return UnprocessableEntity(new { source.Request, mapped.Value });
            }
            var web = WebMapper.MapFrom<UserSearchRequest, WebInteractive>(source.Request);
            if (web == null)
            {
                await _repo.Append(jdbc.SearchTargetTypes.Status, uniqueId, "ERROR: Search request could not be mapped to web request.");
                return StatusCode(500, "Unable to construct web interactive search");
            }
            TaskScheduler uiScheduler = TaskScheduler.Default;
            _ = Task.Factory.StartNew(async () =>
            {
                var response = web.Fetch();
                if (response != null)
                {
                    var responseSaved = await _repo.Append(jdbc.SearchTargetTypes.Response, uniqueId, response);
                    if (responseSaved.Key) await _repo.Complete(uniqueId);
                }
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            return Ok(new { SearchId = uniqueId });
        }
    }
}

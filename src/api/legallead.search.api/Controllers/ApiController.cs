using legallead.jdbc.interfaces;
using legallead.permissions.api.Model;
using legallead.models.Search;
using Microsoft.AspNetCore.Mvc;
using legallead.records.search.Classes;

namespace legallead.search.api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IUserSearchRepository _repo;
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
                return UnprocessableEntity(source.Request);
            }            
            js.Name = uniqueId;
            var mapped = await _repo.Append(jdbc.SearchTargetTypes.Detail, uniqueId, js);
            if (!mapped.Key)
            {
                return UnprocessableEntity(new { source.Request, mapped.Value });
            }
            var web = WebMapper.MapFrom<UserSearchRequest, WebInteractive>(source.Request);
            if (web == null)
            {
                return StatusCode(500, "Unable to construct web interactive search");
            }
            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _ = Task.Factory.StartNew(async () =>
            {
                var response = web.Fetch();
                if (response != null)
                {
                    var responseSaved = await _repo.Append(jdbc.SearchTargetTypes.Response, uniqueId, response);
                    if (responseSaved.Key) await _repo.Complete(uniqueId);
                }
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            return Ok(source);
        }
    }
}

using legallead.permissions.api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController(IAppSettingService svc) : ControllerBase
    {
        private readonly IAppSettingService _svc = svc;

        [HttpPost("appkey")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<AppSettingResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AppSetting([FromBody] AppSettingRequest request)
        {
            var applicationCheck = Request.Validate("invalid application key.");
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var response = new AppSettingResponse { KeyName = request.KeyName };
            var keyvalue = _svc.FindKey(request.KeyName);
            if (string.IsNullOrEmpty(keyvalue)) { return NoContent(); }
            response.KeyValue = keyvalue;
            return Ok(response);
        }
    }
}

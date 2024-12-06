using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("/db")]
    [ApiController]
    public class DbController(
    ILeadAuthenicationService lead) : ControllerBase
    {
        private readonly ILeadAuthenicationService _leadService = lead;

        [HttpPost("begin")]
        public async Task<IActionResult> BeginAsync(BeginDataRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                return Ok();
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteAsync(CompleteDataRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                return Ok();
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        [HttpPost("find")]
        public async Task<IActionResult> FindAsync(FindDataRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                return Ok();
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync(UploadDataRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                return Ok();
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        private const string UserAccountAccess = "user account access credential";

    }
}
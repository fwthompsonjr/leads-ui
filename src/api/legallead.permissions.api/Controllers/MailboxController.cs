using legallead.permissions.api.Entities;
using legallead.permissions.api.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MailboxController(ISearchInfrastructure search, IUserMailbox mailbox) : ControllerBase
    {
        private readonly IUserMailbox _mailDb = mailbox;
        private readonly ISearchInfrastructure _userDb = search;



        [HttpPost("message-count")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<EmailCountBo>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCountAsync(MailboxRequest request)
        {
            var isValid = request.IsValid(_userDb, Request);
            if (!isValid) return BadRequest();
            var response = await _mailDb.GetCountAsync(request);
            return Ok(response);
        }

        [HttpPost("message-body")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<EmailBodyBo>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBodyAsync(MailboxRequest request)
        {
            var isValid = request.IsValid(_userDb, Request);
            if (!isValid) return BadRequest();
            var response = await _mailDb.GetBodyAsync(request);
            return Ok(response);
        }

        [HttpPost("message-list")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<List<EmailListBo>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMailMessagesAsync(MailboxRequest request)
        {
            var isValid = request.IsValid(_userDb, Request);
            if (!isValid) return BadRequest();
            var response = await _mailDb.GetMailMessagesAsync(request);
            return Ok(response);
        }
    }
}
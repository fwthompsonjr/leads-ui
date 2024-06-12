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
        public async Task<IActionResult> GetCount(MailboxRequest request)
        {
            var isValid = request.IsValid(_userDb, Request);
            if (!isValid) return BadRequest();
            var response = await _mailDb.GetCount(request);
            return Ok(response);
        }

        [HttpPost("message-body")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<EmailBodyBo>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBody(MailboxRequest request)
        {
            var isValid = request.IsValid(_userDb, Request);
            if (!isValid) return BadRequest();
            var response = await _mailDb.GetBody(request);
            return Ok(response);
        }

        [HttpPost("message-list")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<List<EmailListBo>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMailMessages(MailboxRequest request)
        {
            var isValid = request.IsValid(_userDb, Request);
            if (!isValid) return BadRequest();
            var response = await _mailDb.GetMailMessages(request);
            return Ok(response);
        }
    }
}
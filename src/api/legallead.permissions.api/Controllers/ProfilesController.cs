using legallead.jdbc.entities;
using legallead.permissions.api;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace legallead.Profiles.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileInfrastructure _db;
        public ProfilesController(IProfileInfrastructure db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("edit-contact-address")]
        public async Task<IActionResult> ChangeContactAddress(ChangeContactAddressRequest[] request)
        {
            var verification = await VerifyRequest(request);
            if (verification.Result != null) return verification.Result;
            var response = await _db.ChangeContactAddress(verification.User, request);
            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        [HttpPost]
        [Route("edit-contact-email")]
        public async Task<IActionResult> ChangeContactEmail(ChangeContactEmailRequest[] request)
        {
            var verification = await VerifyRequest(request);
            if (verification.Result != null) return verification.Result;
            var response = await _db.ChangeContactEmail(verification.User, request);
            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        [HttpPost]
        [Route("edit-contact-name")]
        public async Task<IActionResult> ChangeContactName(ChangeContactNameRequest[] request)
        {
            var verification = await VerifyRequest(request);
            if (verification.Result != null) return verification.Result;
            var response = await _db.ChangeContactName(verification.User, request);
            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        [HttpPost]
        [Route("edit-contact-phone")]
        public async Task<IActionResult> ChangeContactPhone(ChangeContactPhoneRequest[] request)
        {
            var verification = await VerifyRequest(request);
            if (verification.Result != null) return verification.Result;
            var response = await _db.ChangeContactPhone(verification.User, request);
            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }

        private async Task<ActionUserResponse> VerifyRequest(object[] request)
        {
            var response = new ActionUserResponse();
            var user = await _db.GetUser(Request);
            if (user == null)
            {
                response.Result = Unauthorized("Invalid user account.");
                return response;
            }
            response.User = user;
            var validation = BulkValidate(request, out var isValid);
            if (!isValid && validation != null)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                response.Result = BadRequest(messages);
                return response;
            }
            return response;
        }

        private static List<ValidationResult> BulkValidate(object[] collection, out bool isvalid)
        {
            var results = new List<ValidationResult>();
            foreach (var item in collection)
            {
                var resp = item.Validate(out var _);
                if (resp != null) { results.AddRange(resp); }
            }
            isvalid = results.Any();
            return results;
        }

        private sealed class ActionUserResponse
        {
            public IActionResult? Result { get; set; }
            public User? User { get; set; }
        }
    }
}

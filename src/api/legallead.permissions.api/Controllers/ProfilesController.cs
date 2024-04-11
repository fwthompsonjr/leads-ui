using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.permissions.api.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            GetVerification = new ProfileRequestVerification(_db);
        }

        internal IProfileRequestVerification GetVerification { get; set; }

        [HttpPost]
        [Route("get-contact-identity")]
        public async Task<IActionResult> GetContactIdentity()
        {
            var fallback = new GetContactResponse[] {
                new() { ResponseType = "Error", Message = "Unable to retrieve user detail" }
                };
            var user = await _db.GetUser(Request);
            if (user == null)
            {
                fallback[0].Message = "Invalid user account.";
                return Unauthorized(fallback);
            }
            var roleName = await _db.GetContactRole(user);
            var roleDescription = RoleDescriptions.GetDescription(roleName);
            var useritem = new
            {
                userName = user.UserName,
                email = user.Email,
                created = user.CreateDate.GetValueOrDefault(DateTime.UtcNow).ToShortDateString(),
                role = roleName,
                roleDescription
            };
            return Ok(useritem);
        }

        [HttpPost]
        [Route("get-contact-detail")]
        public async Task<IActionResult> GetContactDetail(GetContactRequest request)
        {
            const string noDetailMessage = "Unable to retrieve user detail";
            var fallback = new GetContactResponse[] {
                new() { ResponseType = "Error", Message = noDetailMessage }
                };
            var user = await _db.GetUser(Request);
            if (user == null)
            {
                fallback[0].Message = "Invalid user account.";
                return Unauthorized(fallback);
            }
            var response = await _db.GetContactDetail(user, request.RequestType ?? string.Empty);
            var failure = response?.ToList().Find(a => !a.IsOK);
            if (response != null && failure == null)
                return Ok(response);
            fallback[0].Message = failure?.Message ?? noDetailMessage;
            return Conflict(fallback);
        }

        [HttpPost]
        [Route("edit-contact-address")]
        public async Task<IActionResult> ChangeContactAddress(ChangeContactAddressRequest[] request)
        {
            var verification = await GetVerification.VerifyRequest(Request, request);
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
            var verification = await GetVerification.VerifyRequest(Request, request);
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
            var verification = await GetVerification.VerifyRequest(Request, request);
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
            var verification = await GetVerification.VerifyRequest(Request, request);
            if (verification.Result != null) return verification.Result;
            var response = await _db.ChangeContactPhone(verification.User, request);
            if (response.Key)
                return Ok(response);

            return Conflict(response);
        }
    }
}
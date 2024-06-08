using legallead.permissions.api.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [Route("get-contact-index")]
        public async Task<IActionResult> GetContactId(GetContactRequest request)
        {
            var fallback = new GetContactResponse[] {
                new() { ResponseType = "Error", Message = "Unable to retrieve user detail" }
                };
            var current = fallback[0];
            if (!(request.RequestType ?? string.Empty).Equals("UserId", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(current);
            }
            var user = await _db.GetUser(Request);
            if (user == null)
            {
                current.Message = "Invalid user account.";
                return Unauthorized(current);
            }
            current.IsOK = true;
            current.ResponseType = "Success";
            current.Message = user.Id;
            return Ok(current);
        }

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
        [ServiceFilter(typeof(ProfileChanged))]
        public async Task<IActionResult> ChangeContactAddress(ChangeContactAddressRequest[] request)
        {
            var verification = await GetVerification.VerifyRequest(Request, request);
            if (verification.Result != null) return verification.Result;
            if (BlankProfileHandler.SubstituteBlankValues(request.GetType(), request) is not ChangeContactAddressRequest[] changed)
            {
                return UnprocessableEntity(request);
            }
            var response = await _db.ChangeContactAddress(verification.User, changed);
            if (response.Key)
            {
                var serilized = GetChangeResponse("Address",
                    response.Value,
                    verification.User, changed);
                return Ok(serilized);
            }

            return Conflict(response);
        }

        [HttpPost]
        [Route("edit-contact-email")]
        [ServiceFilter(typeof(ProfileChanged))]
        public async Task<IActionResult> ChangeContactEmail(ChangeContactEmailRequest[] request)
        {
            var verification = await GetVerification.VerifyRequest(Request, request);
            if (verification.Result != null) return verification.Result;
            if (BlankProfileHandler.SubstituteBlankValues(request.GetType(), request) is not ChangeContactEmailRequest[] changed)
            {
                return UnprocessableEntity(request);
            }
            var response = await _db.ChangeContactEmail(verification.User, changed);
            if (response.Key)
            {
                var serilized = GetChangeResponse("Email",
                    response.Value,
                    verification.User, changed);
                return Ok(serilized);
            }


            return Conflict(response);
        }

        [HttpPost]
        [Route("edit-contact-name")]
        [ServiceFilter(typeof(ProfileChanged))]
        public async Task<IActionResult> ChangeContactName(ChangeContactNameRequest[] request)
        {
            var verification = await GetVerification.VerifyRequest(Request, request);
            if (verification.Result != null) return verification.Result;
            if (BlankProfileHandler.SubstituteBlankValues(request.GetType(), request) is not ChangeContactNameRequest[] changed)
            {
                return UnprocessableEntity(request);
            }
            var response = await _db.ChangeContactName(verification.User, changed);
            if (response.Key)
            {
                var serilized = GetChangeResponse("Name",
                    response.Value,
                    verification.User, changed);
                return Ok(serilized);
            }

            return Conflict(response);
        }

        [HttpPost]
        [Route("edit-contact-phone")]
        [ServiceFilter(typeof(ProfileChanged))]
        public async Task<IActionResult> ChangeContactPhone(ChangeContactPhoneRequest[] request)
        {
            var verification = await GetVerification.VerifyRequest(Request, request);
            if (verification.Result != null) return verification.Result;
            if (BlankProfileHandler.SubstituteBlankValues(request.GetType(), request) is not ChangeContactPhoneRequest[] changed)
            {
                return UnprocessableEntity(request);
            }
            var response = await _db.ChangeContactPhone(verification.User, changed);
            if (response.Key)
            {
                var serilized = GetChangeResponse("Phone",
                    response.Value,
                    verification.User, changed);
                return Ok(serilized);
            }

            return Conflict(response);
        }


        private static KeyValuePair<bool, string> GetChangeResponse(
            string changeName,
            string message,
            User? user,
            object original)
        {
            var js = JsonConvert.SerializeObject(original);
            var data = new
            {
                Email = user?.Email ?? string.Empty,
                Name = changeName,
                Message = message,
                JsonData = js
            };
            js = JsonConvert.SerializeObject(data);
            return new(true, js);
        }
    }
}
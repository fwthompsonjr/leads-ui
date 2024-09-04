using legallead.permissions.api.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Utility
{
    public class ProfileRequestVerification(IProfileInfrastructure db) : IProfileRequestVerification
    {
        private readonly IProfileInfrastructure _db = db;

        public async Task<ActionUserResponse> VerifyRequestAsync(HttpRequest http, object[] request)
        {
            var response = new ActionUserResponse();
            var user = await _db.GetUserAsync(http);
            if (user == null)
            {
                response.Result = new UnauthorizedObjectResult("Invalid user account.");
                return response;
            }
            response.User = user;
            var validation = BulkValidate(request, out var _);
            if (validation.Count != 0)
            {
                var messages = validation.Select(x => x.ErrorMessage).ToList();
                response.Result = new BadRequestObjectResult(messages);
                return response;
            }
            return response;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested through public member")]
        private static List<ValidationResult> BulkValidate(object[] collection, out bool isvalid)
        {
            var results = new List<ValidationResult>();
            foreach (var item in collection)
            {
                var resp = item.Validate(out var _);
                if (resp != null && resp.Count != 0) { results.AddRange(resp); }
            }
            isvalid = results.Count != 0;
            return results;
        }

    }
}

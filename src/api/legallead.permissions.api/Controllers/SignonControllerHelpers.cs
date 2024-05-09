using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace legallead.permissions.api.Controllers
{
    public partial class SignonController
    {


        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private static User MapFromChangePassword(UserChangePasswordModel usersdata, UserModel model, User? user)
        {
            model.Password = usersdata.NewPassword;
            if (user != null)
            {
                model.Email = user.Email;
                model.UserName = user.UserName;
            }
            var update = UserModel.ToUser(model);
            if (!string.IsNullOrEmpty(user?.Id)) update.Id = user.Id;
            return update;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private async Task<IActionResult?> IsViolationIncidentCreated(KeyValuePair<bool, User?> validUser)
        {
            var user = validUser.Value;
            if (validUser.Key && user != null && !string.IsNullOrEmpty(user.Id))
            {
                return null;
            }
            if (!string.IsNullOrEmpty(user?.Id))
            {
                await _lockingDb.AddIncident(user.Id);
            }
            await _logSvc.LogWarning("Failed : Validate User Credential. Returning 401 - Unauthorized");
            return Unauthorized("Invalid username or password...");
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private async Task<IActionResult?> IsAccountLockedViolation(string userId, string email)
        {
            var isLocked = await _lockingDb.IsAccountLocked(userId);
            if (isLocked)
            {
                await _logSvc.LogWarning("Failed : Account is locked. Returning 403 - Forbidden");
                var message = new { Email = email, Message = "Account is locked. Contact system administrator to unlock." };
                return StatusCode((int)HttpStatusCode.Forbidden, message);
            }
            return null;
        }
    }
}

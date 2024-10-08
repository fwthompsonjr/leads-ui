﻿
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController(DataProvider db, IStateSearchProvider stateSearch) : ControllerBase
    {
        private const string defaultReadme = "No ReadMe information is available.";
        private static bool isReadMeBuilt = false;

        private static readonly object _instance = new();
        private static string? _readme;
        private readonly DataProvider _db = db;
        private readonly IStateSearchProvider _searchProvider = stateSearch;

        [HttpGet]
        [Route("read-me")]
        public string ReadMe()
        {
            if (_readme == null)
            {
                SetApplicationReadMe(defaultReadme);
                GenerateReadMe(ref _readme);
            }
            return _readme ?? defaultReadme;
        }

        [HttpGet]
        [Route("apps")]
        public async Task<IEnumerable<ApplicationModel>?> ListAsync()
        {
            try
            {
                var response = await _db.ComponentDb.GetAll();
                if (response == null || !response.Any()) { return ApplicationsFallback; }
                var apps = response.Select(s =>
                new ApplicationModel
                {
                    Id = s.Id,
                    Name = s.Name ?? string.Empty
                });
                return apps;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return ApplicationsFallback;
        }

        [HttpPost]
        [Route("register")]
        [ServiceFilter(typeof(RegistrationCompleted))]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterAccountModel model)
        {
            var response = "An error occurred registering account.";
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return BadRequest(response);
            }
            var registration = await RegisterAsync(Request, model, response);
            return registration;
        }

        [HttpGet]
        [Route("state-configuration")]
        public IActionResult StateList()
        {
            var data = _searchProvider.GetStates();
            return Ok(data);
        }

        [ExcludeFromCodeCoverage(Justification = "Private method accessing public tested members")]
        private async Task<IActionResult> RegisterAsync(HttpRequest request, RegisterAccountModel model, string response)
        {
            var registration = await RegisterUserAsync(request, model, response);
            if (registration is IActionResult action) return action;
            if (registration is not User user) return UnprocessableEntity();
            var accountResult = await RegisterUserAccountAsync(user, response);
            return accountResult;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method accessing public tested members")]
        private async Task<object> RegisterUserAsync(HttpRequest request, RegisterAccountModel model, string response)
        {
            var applicationCheck = request.Validate(response);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var account = new UserModel
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
            };
            var user = UserModel.ToUser(account);
            var isDuplicate = await IsDuplicateAccountAsync(user);
            if (user == null || isDuplicate)
            {
                return Conflict("Potential duplicate account found.");
            }
            return user;
        }
        [ExcludeFromCodeCoverage(Justification = "Private method accessing public tested members")]
        private async Task<IActionResult> RegisterUserAccountAsync(User user, string response)
        {
            try
            {
                var isAdded = await TryCreateAccountAsync(user);
                var aresponse = isAdded ? user.Id : response;
                if (isAdded)
                {
                    await _db.InitializeProfileAsync(user);
                    var initOk = (await _db.InitializePermissionAsync(user));
                    await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.AccountRegistrationCompleted);
                    await _db.ProfileHistoryDb.CreateSnapshot(user, jdbc.ProfileChangeTypes.AccountRegistrationCompleted);
                    if (initOk)
                    {
                        await _db.SetPermissionGroupAsync(user, "Guest");
                    }
                }
                if (aresponse != null) return Ok(aresponse);
                return UnprocessableEntity(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [ExcludeFromCodeCoverage(Justification = "Private method accessing public tested members")]
        private static void GenerateReadMe(ref string? readme)
        {
            if (isReadMeBuilt) return;
            readme = Properties.Resources.README;
            isReadMeBuilt = true;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method accessing public tested members")]
        private async Task<bool> TryCreateAccountAsync(User user)
        {
            try
            {
                user.CreateDate = DateTime.UtcNow;
                await _db.UserDb.Create(user);
                return true;
            }
            catch (Exception ex)
            {
                var eventId = new EventId((int)ErrorCodes.CreateAccountFailure, ErrorCodes.CreateAccountFailure.ToString());
                Console.WriteLine(eventId);
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private method accessing public tested members")]
        private async Task<bool> IsDuplicateAccountAsync(User user)
        {
            try
            {
                var emailMatch = await _db.UserDb.GetByEmail(user.Email);
                if (emailMatch != null) { return true; }
                var nameMatch = await _db.UserDb.GetByName(user.UserName);
                if (nameMatch != null) { return true; }
                return false;
            }
            catch (Exception ex)
            {
                var eventId = new EventId((int)ErrorCodes.CheckForDuplicateAccount, ErrorCodes.CheckForDuplicateAccount.ToString());
                Console.WriteLine(eventId);
                Console.WriteLine(ex.ToString());
                return true;
            }
        }

        private static List<ApplicationModel> ApplicationsFallback
            => applications ??= GetApplicationsFallback();

        private static List<ApplicationModel>? applications;

        private static List<ApplicationModel> GetApplicationsFallback()
        {
            return ApplicationModel.GetApplicationsFallback();
        }

        private static void SetApplicationReadMe(string fallback)
        {
            lock (_instance)
            {
                _readme = _readme.IfNull(fallback);
            }
        }
    }
}
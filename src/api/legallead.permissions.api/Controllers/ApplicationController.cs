using legallead.jdbc.entities;
using legallead.jdbc.models;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private const string defaultReadme = "No ReadMe information is available.";
        private static bool isReadMeBuilt = false;

        private static readonly object _instance = new();
        private static string? _readme;
        private readonly DataProvider _db;

        public ApplicationController(DataProvider db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("read-me")]
        public string ReadMe()
        {
            if (_readme == null)
            {
                _readme ??= defaultReadme;
                GenerateReadMe(ref _readme);
            }
            return _readme ?? defaultReadme;
        }

        [HttpGet]
        [Route("apps")]
        public async Task<IEnumerable<ApplicationModel>?> List()
        {
            var response = await _db.ComponentDb.GetAll();
            if (response == null) { return null; }
            var apps = response.Select(s => 
            new ApplicationModel
            { 
                Id = s.Id ?? string.Empty, 
                Name = s.Name ?? string.Empty
            });
            return apps;
        }

        [HttpPost]
        [Route("register")]
        public async Task<string> Register([FromBody] RegisterAccountModel model)
        {
            var response = "An error occurred registering account.";
            var merrors = model.Validate(out bool isModelValid);
            if (!isModelValid)
            {
                response = string.Join(';', merrors.Select(m => m.ErrorMessage));
                return response;
            }
            var applicationCheck = Request.Validate(_db, response);
            if (!applicationCheck.Key) { return applicationCheck.Value; }
            var account = new UserModel
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
            };
            var user = UserModel.ToUser(account);
            var isDuplicate = await IsDuplicateAccount(user);
            if (user == null || isDuplicate)
            {
                return "Potential duplicate account found.";
            }
            var isAdded = await TryCreateAccount(user);
            var aresponse = isAdded ? user.Id : response;
            if (isAdded)
            {
                await _db.InitializeProfile(user);
                await _db.InitializePermission(user);
            }
            return aresponse ?? response;
        }

        private static void GenerateReadMe(ref string readme)
        {
            if (isReadMeBuilt) return;
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null || assembly.Location == null) return;
            var execName = new Uri(assembly.Location).AbsolutePath;
            if (execName != null && System.IO.File.Exists(execName))
            {
                var contentRoot = Path.GetDirectoryName(execName) ?? "";
                var dataRoot = Path.Combine(contentRoot, "_db");
                var dataFile = Path.Combine(dataRoot, "readme.txt");
                if (System.IO.File.Exists(dataFile))
                {
                    lock (_instance)
                    {
                        readme = System.IO.File.ReadAllText(dataFile);
                        isReadMeBuilt = true;
                    }
                }
                else
                {
                    readme = defaultReadme;
                }
            }
        }

        private async Task<bool> TryCreateAccount(User user)
        {
            try
            {
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

        private async Task<bool> IsDuplicateAccount(User user)
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
    }
}
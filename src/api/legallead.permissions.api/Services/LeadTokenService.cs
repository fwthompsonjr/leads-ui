using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace legallead.permissions.api.Services
{
    public static class LeadTokenService
    {
        private static readonly Encoding encoding = Encoding.UTF8;
        public static string GenerateToken(string reason, LeadUserModel user)
        {
            var data = new List<byte>();
            var token = GetToken();
            var model = new LeadUserSecurityBo { Key = token.Key, Reason = reason, User = user };
            var _time = encoding.GetBytes(token.ExpirationDate.ToString("s"));
            var _user = encoding.GetBytes(JsonConvert.SerializeObject(model));

            data.AddRange(_time);
            data.AddRange(_user);

            return Convert.ToBase64String(data.ToArray());
        }

        public static LeadUserSecurityBo? GetModel(string token, out DateTime? expirationDate)
        {
            expirationDate = null;
            try
            {
                var position = DateTime.UtcNow.ToString("s").Length;
                var data = Convert.FromBase64String(token);
                var expiry = encoding.GetString(data.Take(position).ToArray());
                var serialized = encoding.GetString(data.Skip(position).ToArray());
                var model = serialized.ToInstance<LeadUserSecurityBo>();
                if (model == null) return null;
                if (!DateTime.TryParseExact(expiry, "s", enUS, DateTimeStyles.None, out var dateValue)) return null;
                expirationDate = dateValue;
                return model;
            }
            catch
            {
                return null;
            }
        }

        public static TokenValidationModel GetValidationModel(string token, string reason)
        {
            var response = new TokenValidationModel();
            var model = GetModel(token, out DateTime? expirationDate);
            return ValidateModel(reason, response, model, expirationDate);
        }

        [ExcludeFromCodeCoverage]
        private static TokenValidationModel ValidateModel(string reason, TokenValidationModel response, LeadUserSecurityBo? model, DateTime? expirationDate)
        {
            if (expirationDate == null)
            {
                response.Errors.Add(Enumerations.TokenValidationStatus.Expired);
            }
            if (model == null)
            {
                response.Errors.Add(Enumerations.TokenValidationStatus.WrongUser);
                return response;
            }
            if (!FindToken(model.Key))
            {
                response.Errors.Add(Enumerations.TokenValidationStatus.WrongGuid);
            }
            if (expirationDate != null && expirationDate < DateTime.UtcNow)
            {
                response.Errors.Add(Enumerations.TokenValidationStatus.Expired);
            }
            if (string.IsNullOrEmpty(model.Reason) || !model.Reason.Equals(reason))
            {
                response.Errors.Add(Enumerations.TokenValidationStatus.WrongPurpose);
            }
            return response;
        }

        private static bool FindToken(Guid securityKey)
        {
            lock (locker)
            {
                SecurityList.RemoveAll(x => x.ExpirationDate < DateTime.UtcNow);
                return SecurityList.Exists(s => s.Key.Equals(securityKey));
            }
        }

        private static SecurityItem GetToken()
        {
            lock (locker)
            {
                var item = new SecurityItem();
                SecurityList.Add(item);
                SecurityList.RemoveAll(x => x.ExpirationDate < DateTime.UtcNow);
                return item;
            }
        }

        private static readonly List<SecurityItem> SecurityList = [];

        private sealed class SecurityItem
        {
            public SecurityItem()
            {
                ExpirationDate = DateTime.UtcNow.AddMinutes(30);
            }
            public Guid Key { get; } = Guid.NewGuid();
            public DateTime ExpirationDate { get; }
        }
        private static readonly CultureInfo enUS = new("en-US");
        private static readonly object locker = new();
    }
}

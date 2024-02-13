using legallead.jdbc.entities;
using System.Diagnostics.CodeAnalysis;

namespace legallead.jdbc.models
{
    public class UserModel
    {
        private const string saltLocal = "legal.lead.user.passcode";
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public static User ToUser(UserModel model)
        {
            var pword = $"{model.UserName}|{model.Password}";
            var encoded = CryptoManager.Encrypt(pword, saltLocal, out var vector);
            return new User
            {
                Id = Guid.NewGuid().ToString("D"),
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = encoded,
                PasswordSalt = vector
            };
        }

        [ExcludeFromCodeCoverage(Justification = "Test coverage through integration tests")]
        public static bool IsPasswordMatched(string password, User model)
        {
            try
            {
                var pipe = '|';
                var decoded = CryptoManager.Decrypt(model.PasswordHash, saltLocal, model.PasswordSalt);
                if (!decoded.Contains(pipe)) { return false; }
                var pword = decoded.Split(pipe)[1];
                return pword.Equals(password);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
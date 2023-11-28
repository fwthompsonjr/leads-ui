using legallead.jdbc.entities;

namespace legallead.jdbc.models
{
    internal class UserModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public static User ToUser(UserModel model)
        {
            const string saltLocal = "legal.lead.user.passcode";
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
    }
}
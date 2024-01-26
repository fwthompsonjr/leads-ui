using legallead.desktop.entities;
using legallead.desktop.interfaces;

namespace legallead.desktop.extensions
{
    internal static class UserExtensions
    {
        public static void AppendAuthorization(this HttpClient client, UserBo user)
        {
            if (user.Token == null || string.IsNullOrEmpty(user.Token.AccessToken) || !user.IsAuthenicated) return;
            var token = user.Token.AccessToken;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public static async Task ExtendToken(this UserBo user, IPermissionApi? api)
        {
            if (api == null) return;
            if (user.Token == null || string.IsNullOrEmpty(user.Token.AccessToken)) return;
            var expiration = DateTime.UtcNow.Subtract(user.Token.Expires.GetValueOrDefault());
            if (expiration.TotalSeconds < -45) return;
            var token = user.Token;
            var payload = new { refreshToken = token.RefreshToken, accessToken = token.AccessToken };
            var response = await api.Post("refresh", payload, user);
            if (response == null || response.StatusCode != 200)
            {
                user.Token = null;
                return;
            }
            var newtoken = ObjectExtensions.TryGet<AccessTokenBo>(response.Message);
            if (!newtoken.Expires.HasValue)
            {
                user.Token = null;
                return;
            }
            user.Token = newtoken;
        }
    }
}
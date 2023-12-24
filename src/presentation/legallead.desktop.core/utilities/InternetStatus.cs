using System.Net.NetworkInformation;
using System.Runtime.Caching;
using legallead.desktop.interfaces;

namespace legallead.desktop.utilities
{
    internal class InternetStatus : IInternetStatus
    {
        public bool GetConnectionStatus()
        {
            var memoryCache = MemoryCache.Default;

            if (!memoryCache.Contains(IsConnectedKeyName))
            {
                var expiration = DateTimeOffset.UtcNow.AddMinutes(5);
                memoryCache.Add(IsConnectedKeyName, IsConnectedToInternet(), expiration);
            }

            return ConvertResponse(memoryCache.Get(IsConnectedKeyName));
        }

        private static bool ConvertResponse(object? response)
        {
            if (response == null) return false;
            return Convert.ToBoolean(response);
        }

        private static bool IsConnectedToInternet()
        {
            string host = "https://www.google.com";
            bool result = false;
            Ping p = new();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch
            {
                return result;
            }
            return result;
        }

        private const string IsConnectedKeyName = "internet-connection-status";
    }
}
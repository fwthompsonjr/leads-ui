namespace legallead.permissions.api.Services
{
    internal static class PaymentCodeService
    {

        public static string? GetCode(string key)
        {
            lock (locker)
            {
                if (string.IsNullOrEmpty(key)) return null;
                var found = PaymentCodes.TryGetValue(key, out var result);
                if (!found) return null;
                return result;
            }
        }

        public static void SetCode(string key, string code)
        {
            lock (locker)
            {
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(code)) return;
                _ = PaymentCodes.TryGetValue(key, out var result);
                if (result != null && result.Equals(code)) return;
                PaymentCodes.Add(key, code);
            }
        }
        private static readonly Dictionary<string, string> PaymentCodes = [];
        private static readonly object locker = new();
    }
}

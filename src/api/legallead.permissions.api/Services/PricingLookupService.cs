using legallead.jdbc.entities;

namespace legallead.permissions.api.Services
{
    internal static class PricingLookupService
    {
        private static readonly object sync = new();

        private static readonly List<PricingCodeBo> _lookup = new();
        public static List<PricingCodeBo> PricingCodes => _lookup;

        public static void Append(List<PricingCodeBo> codes)
        {
            codes.ForEach(Append);
        }

        private static void Append(PricingCodeBo code)
        {
            lock (sync)
            {
                var found = _lookup.Find(x => x.Id == code.Id);
                if (found != null) return;
                _lookup.RemoveAll(x => x.KeyName == code.KeyName);
                _lookup.Add(code);
            }
        }

    }
}

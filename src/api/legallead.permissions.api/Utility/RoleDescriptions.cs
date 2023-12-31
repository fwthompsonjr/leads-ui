namespace legallead.permissions.api.Utility
{
    public static class RoleDescriptions
    {
        public static string GetDescription(string name)
        {
            var keys = RoleNames.Keys.ToList();
            var selection = keys.Find(k => k.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? "Guest";
            return RoleNames[selection];
        }

        private static readonly Dictionary<string, string> RoleNames = new()
        {
            { "Guest", Properties.Resources.description_role_guest },
            { "Silver", Properties.Resources.description_role_silver },
            { "Gold", Properties.Resources.description_role_gold },
            { "Platinum", Properties.Resources.description_role_platinum },
            { "Admin", Properties.Resources.description_role_admin },
        };
    }
}
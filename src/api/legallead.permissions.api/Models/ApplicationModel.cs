namespace legallead.permissions.api.Model
{
    public class ApplicationModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        internal static List<ApplicationModel> GetApplicationsFallback()
        {
            var emptyGuid = Guid.Empty.ToString("D");
            return
            [
                new() { Id = emptyGuid, Name = "legallead.permissions.api" },
                new() { Id = emptyGuid, Name = "oxford.leads.data.services" }
            ];
        }
    }
}
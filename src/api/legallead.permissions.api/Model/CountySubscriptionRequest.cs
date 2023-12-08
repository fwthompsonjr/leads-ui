using legallead.json.db.attr;

namespace legallead.permissions.api.Model
{
    public class CountySubscriptionRequest
    {
        [UsCounty]
        [StateCheck("State")]
        public string? County { get; set; } = string.Empty;

        [UsState]
        public string? State { get; set; } = string.Empty;
    }
}

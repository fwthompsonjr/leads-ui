using legallead.json.db.attr;

namespace legallead.permissions.api.Model
{
    public class StateSubscriptionRequest
    {
        [UsState]
        public string? Name { get; set; } = string.Empty;
    }
}

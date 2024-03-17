namespace legallead.permissions.api.Models
{
    public class SubscriptionCreatedModel
    {
        public string Id { get; set; } = Guid.Empty.ToString("D");
        public string Url { get; set; } = "ERROR";
    }
}

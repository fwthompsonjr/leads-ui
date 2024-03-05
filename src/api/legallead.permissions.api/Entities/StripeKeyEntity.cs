namespace legallead.permissions.api.Entities
{
    public class StripeKeyEntity
    {
        public string ActiveName { get; set; } = "test";
        public string? WebhookId { get; set; }
        public List<StripeKeyItem> Items { get; set; } = new();

        public string GetActiveName()
        {
            var item = Items.Find(x => x.Name == ActiveName);
            return item?.Value ?? string.Empty;
        }
    }
}

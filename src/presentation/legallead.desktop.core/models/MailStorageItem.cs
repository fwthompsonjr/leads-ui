namespace legallead.desktop.models
{
    public class MailStorageItem
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? FromAddress { get; set; }
        public string? ToAddress { get; set; }
        public string? Subject { get; set; }
        public string? CreateDate { get; set; }
    }
}

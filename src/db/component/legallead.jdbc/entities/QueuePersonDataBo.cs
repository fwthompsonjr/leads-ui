namespace legallead.jdbc.entities
{
    public class QueuePersonDataBo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}

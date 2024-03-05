namespace legallead.jdbc.entities
{
    public class PaymentCustomerBo
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public string? Email { get; set; }
        public bool? IsTest { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}

using Dapper;

namespace legallead.jdbc.entities
{
    public class PaymentCustomerQuery
    {
        public string? UserId { get; set; }
        public string? AccountType { get; set; }

        internal virtual DynamicParameters GetParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("user_index", UserId);
            parameters.Add("account_type", AccountType);
            return parameters;
        }
    }
}

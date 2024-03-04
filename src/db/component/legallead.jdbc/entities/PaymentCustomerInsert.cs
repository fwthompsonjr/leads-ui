using Dapper;

namespace legallead.jdbc.entities
{
    public class PaymentCustomerInsert : PaymentCustomerQuery
    {
        public string? CustomerId { get; set; }
        internal override DynamicParameters GetParameters()
        {
            var parameters = base.GetParameters();
            parameters.Add("customer_index", CustomerId);
            return parameters;
        }
    }
}

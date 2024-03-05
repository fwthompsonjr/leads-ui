using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;

namespace legallead.jdbc.implementations
{
    public class CustomerRepository :
        BaseRepository<PaymentCustomerDto>, ICustomerRepository
    {
        public CustomerRepository(DataContext context) : base(context)
        {
        }

        public async Task<bool> DoesCustomerExist(PaymentCustomerQuery query)
        {
            var customer = await GetCustomer(query);
            return customer != null;
        }

        public async Task<PaymentCustomerBo?> GetCustomer(PaymentCustomerQuery query)
        {
            const string prc = "CALL USP_FIND_PAYMENT_CUSTOMER(?, ?);";
            try
            {
                var parms = query.GetParameters();
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<PaymentCustomerDto>(connection, prc, parms);
                if (response == null) return null;
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<PaymentCustomerBo>(json);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<KeyValuePair<bool, string>> AddCustomer(PaymentCustomerInsert dto)
        {
            const string prc = "CALL USP_CREATE_PAYMENT_CUSTOMER(?, ?, ?);";
            try
            {
                var parms = dto.GetParameters();
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, "Customer created successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }
    }
}
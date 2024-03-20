using Dapper;
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

        public async Task<List<UnMappedCustomerBo>?> GetUnMappedCustomers(PaymentCustomerQuery query)
        {
            const string prc = "CALL USP_QUERY_USER_WITHOUT_CUSTOMER_ACCOUNT( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("", query.AccountType);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<CustomerDto>(connection, prc, parms);
                if (response == null) return null;
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<List<UnMappedCustomerBo>>(json);
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

        public async Task<KeyValuePair<bool, string>> AddLevelChangeRequest(string jsonRequest)
        {
            const string prc = "CALL USP_CREATE_LEVELREQUEST( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("pay_load", jsonRequest);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, "Created successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        public async Task<List<LevelRequestBo>?> GetLevelRequestHistory(string userId)
        {
            const string prc = "CALL USP_FIND_LEVELREQUEST_BY_USER_ID( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("user_index", userId);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LevelRequestDto>(connection, prc, parms);
                if (response == null) return null;
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<List<LevelRequestBo>>(json);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<LevelRequestBo?> GetLevelRequestById(string externalId)
        {
            const string prc = "CALL USP_FIND_LEVELREQUEST_BY_EXTERNAL_INDEX( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("external_id", externalId);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<LevelRequestDto>(connection, prc, parms);
                if (response == null) return null;
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<LevelRequestBo>(json);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateLevelChangeRequest(string jsonRequest)
        {
            const string prc = "CALL USP_UPDATE_LEVELREQUEST( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("pay_load", jsonRequest);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                await SynchronizeUserSubscriptions();
                return new(true, "Created successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }



        public async Task<KeyValuePair<bool, string>> AddDiscountChangeRequest(string jsonRequest)
        {
            const string prc = "CALL USP_CREATE_DISCOUNTREQUEST( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("pay_load", jsonRequest);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, "Created successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }
        public async Task<List<LevelRequestBo>?> GetDiscountRequestHistory(string userId)
        {
            const string prc = "CALL USP_FIND_DISCOUNTREQUEST_BY_USER_ID( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("user_index", userId);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LevelRequestDto>(connection, prc, parms);
                if (response == null) return null;
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<List<LevelRequestBo>>(json);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LevelRequestBo?> GetDiscountRequestById(string externalId)
        {
            const string prc = "CALL USP_FIND_DISCOUNTREQUEST_BY_EXTERNAL_INDEX( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("external_id", externalId);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<LevelRequestDto>(connection, prc, parms);
                if (response == null) return null;
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<LevelRequestBo>(json);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<KeyValuePair<bool, string>> UpdateDiscountChangeRequest(string jsonRequest)
        {
            const string prc = "CALL USP_UPDATE_DISCOUNTREQUEST( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("pay_load", jsonRequest);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                await SynchronizeUserSubscriptions();
                return new(true, "Created successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }
        public async Task<List<SubscriptionDetailBo>?> GetUserSubscriptions(bool forVerification)
        {
            const string prc_all = "CALL USP_GET_USER_SUBSCRIPTIONS();";
            const string prc_verify = "CALL USP_GET_USER_SUBSCRIPTIONS_NEEDING_VERIFICATION();";
            var prc = forVerification ? prc_verify : prc_all;
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<SubscriptionDetailDto>(connection, prc);
                if (response == null) return null;
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<List<SubscriptionDetailBo>>(json);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<KeyValuePair<bool, string>> SynchronizeUserSubscriptions()
        {
            const string prc = "CALL USP_INSERT_USERSUBSCRIPTION();";
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc);
                return new(true, "Created successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateSubscriptionVerification(ISubscriptionDetail source)
        {
            const string prc = "CALL USP_UPDATE_USER_SUBSCRIPTIONS_VERIFICATION( ? );";
            try
            {
                var obj = new { source.Id, source.IsSubscriptionVerified, source.VerificationDate };
                var jsonRequest = JsonConvert.SerializeObject(obj);
                var parms = new DynamicParameters();
                parms.Add("pay_load", jsonRequest);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, "Created successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

    }
}
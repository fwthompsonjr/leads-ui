using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Newtonsoft.Json;

namespace legallead.jdbc.implementations
{
    public class UserMangementRepository(DataContext context) :
        BaseRepository<DbCountyFileDto>(context), IUserMangementRepository
    {

        protected async Task<bool> InitializeAsync(UserManagementRequest request)
        {
            var prc = ProcedureNames.InitProc;
            try
            {
                using var connection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add(requestorIndex, request.RequestId);
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<LeadUserAccountBo>?> GetAccountsAsync(UserManagementRequest request)
        {
            _ = await InitializeAsync(request);
            var prc = ProcedureNames.GetAccountsProc;
            var parameters = new DynamicParameters();
            parameters.Add(requestorIndex, request.RequestId);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LeadUserAccountDto>(connection, prc, parameters);
                if (response == null) return null;
                var js = JsonConvert.SerializeObject(response);
                var data = JsonConvert.DeserializeObject<List<LeadUserAccountBo>>(js);
                return data;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<LeadCountyPricingBo>?> GetPricingAsync(UserManagementRequest request)
        {
            _ = await InitializeAsync(request);
            var prc = ProcedureNames.GetPricingProc;
            var parameters = new DynamicParameters();
            parameters.Add(requestorIndex, request.RequestId);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LeadCountyPricingDto>(connection, prc, parameters);
                if (response == null) return null;
                var js = JsonConvert.SerializeObject(response);
                var data = JsonConvert.DeserializeObject<List<LeadCountyPricingBo>>(js);
                return data;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<LeadUserCountyViewBo>?> GetCountyAsync(UserManagementRequest request)
        {
            _ = await InitializeAsync(request);
            var prc = ProcedureNames.GetCountyProc;
            var parameters = new DynamicParameters();
            parameters.Add(requestorIndex, request.RequestId);
            parameters.Add(userIndex, request.UserId);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LeadUserCountyViewDto>(connection, prc, parameters);
                if (response == null) return null;
                var js = JsonConvert.SerializeObject(response);
                var data = JsonConvert.DeserializeObject<List<LeadUserCountyViewBo>>(js);
                return data;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<LeadUserProfileBo>?> GetProfileAsync(UserManagementRequest request)
        {
            _ = await InitializeAsync(request);
            var prc = ProcedureNames.GetProfileProc;
            var parameters = new DynamicParameters();
            parameters.Add(requestorIndex, request.RequestId);
            parameters.Add(userIndex, request.UserId);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LeadUserProfileDto>(connection, prc, parameters);
                if (response == null) return null;
                var js = JsonConvert.SerializeObject(response);
                var data = JsonConvert.DeserializeObject<List<LeadUserProfileBo>>(js);
                return data;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<LeadUserSearchBo>?> GetSearchAsync(UserManagementRequest request)
        {
            _ = await InitializeAsync(request);
            var prc = ProcedureNames.GetSearchProc;
            var parameters = new DynamicParameters();
            parameters.Add(requestorIndex, request.RequestId);
            parameters.Add(userIndex, request.UserId);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LeadUserSearchDto>(connection, prc, parameters);
                if (response == null) return null;
                var js = JsonConvert.SerializeObject(response);
                var data = JsonConvert.DeserializeObject<List<LeadUserSearchBo>>(js);
                return data;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<LeadUserInvoiceBo>?> GetInvoiceAsync(UserManagementRequest request)
        {
            _ = await InitializeAsync(request);
            var prc = ProcedureNames.GetInvoiceProc;
            var parameters = new DynamicParameters();
            parameters.Add(requestorIndex, request.RequestId);
            parameters.Add(userIndex, request.UserId);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<LeadUserInvoiceDto>(connection, prc, parameters);
                if (response == null) return null;
                var js = JsonConvert.SerializeObject(response);
                var data = JsonConvert.DeserializeObject<List<LeadUserInvoiceBo>>(js);
                return data;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<bool> UpdateProfileAsync(UserManagementRequest request)
        {
            var prc = ProcedureNames.UpdateProfileProc;
            var parameters = new DynamicParameters();
            parameters.Add(requestorIndex, request.RequestId);
            parameters.Add(jsPayload, request.Payload);
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateUsageLimitAsync(UserManagementRequest request)
        {
            var prc = ProcedureNames.UpdateUsageLimitProc;
            var parameters = new DynamicParameters();
            parameters.Add(requestorIndex, request.RequestId);
            parameters.Add(jsPayload, request.Payload);
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private const string requestorIndex = "requestorIndex";
        private const string userIndex = "userIndex";
        private const string jsPayload = "jsPayload";

        private static class ProcedureNames
        {
            public const string InitProc = "CALL ADM_USP_CLONE_LEADUSER_ACCT_TO_USER( ? );";
            public const string GetAccountsProc = "CALL ADM_USP_GET_LEADUSER_ACCT_LIST( ? );";
            public const string GetPricingProc = "CALL ADM_USP_GET_COUNTY_PRICING_LIST( ? );";
            public const string GetCountyProc = "CALL ADM_USP_GET_COUNTY_LIST_BY_USER_ID( ?, ? );";
            public const string GetProfileProc = "CALL ADM_USP_GET_LEADUSER_PROFILE( ?, ? );";
            public const string GetSearchProc = "CALL ADM_USP_GET_LEADUSER_SEARCH_HISTORY( ?, ? );";
            public const string GetInvoiceProc = "CALL ADM_USP_GET_LEADUSER_INVOICE_HISTORY( ?, ? );";
            public const string UpdateProfileProc = "CALL ADM_USP_UPSERT_LEADUSER_PROFILE( ?, ? );";
            public const string UpdateUsageLimitProc = "CALL ADM_USP_UPSERT_LEADUSER_MONTHLY_LIMIT( ?, ? );";
        }
    }
}
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace legallead.jdbc.implementations
{
    public class UserSearchRepository : BaseRepository<SearchDto>, IUserSearchRepository
    {
        public UserSearchRepository(DataContext context) : base(context)
        {
        }

        public async Task<SearchRestrictionDto> GetSearchRestriction(string userId)
        {
            const string prc = "CALL USP_GET_SEARCH_RESTRICTION_PARAMETERS( '{0}' );";
            var command = string.Format(prc, userId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<SearchRestrictionDto>(connection, command);
            return response ?? new();
        }
        public async Task<bool> ExtendRestriction(string userId)
        {
            const string prc = "CALL USP_APPEND_SEARCH_RESTRICTION_MULTIPLIER( '{0}' );";
            try
            {
                var cmd = string.Format(prc, userId);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, cmd);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> RequeueSearches()
        {
            const string prc1 = "CALL PRC__VERIFY_SEARCH_STATUS( );";
            const string prc2 = "CALL USP_SEARCH_QUEUE_RETRY_FAILED_REQUEST( );";
            try
            {
                var commands = new List<string>() { prc2, prc1 };
                using var connection = _context.CreateConnection();
                await Task.Run(() =>
                {
                    commands.ForEach(cmd =>
                    {
                        _command.ExecuteAsync(connection, cmd).GetAwaiter().GetResult();
                    });
                });
                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<IEnumerable<SearchDtoHeader>> History(string userId)
        {
            const string prc = "CALL USP_QUERY_USER_SEARCH( '{0}' );";
            var command = string.Format(prc, userId);
            using var connection = _context.CreateConnection();
            await _command.ExecuteAsync(connection, sprc);
            var response = await _command.QueryAsync<SearchQueryDto>(connection, command);
            var translation = response.Select(x => new SearchDtoHeader
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                EstimatedRowCount = x.ExpectedRows,
                CreateDate = x.CreateDate,
                SearchProgress = x.SearchProgress,
                StateCode = x.StateCode,
                CountyName = x.CountyName
            });
            return translation;
        }
        public async Task<IEnumerable<SearchPreviewBo>> Preview(string searchId)
        {
            const string prc = "CALL USP_GET_SEARCH_RECORD_PREVIEW( '{0}' );";
            var command = string.Format(prc, searchId);
            using var connection = _context.CreateConnection();
            await _command.ExecuteAsync(connection, sprc);
            var response = await _command.QueryAsync<SearchPreviewDto>(connection, command);
            var translation = response.Select(x => TranslateTo<SearchPreviewBo>(x));
            return translation;
        }

        public async Task<IEnumerable<SearchFinalBo>> GetFinal(string searchId)
        {
            var nodata = Array.Empty<SearchFinalBo>();
            const string prc = "CALL USP_GET_SEARCH_RECORD_FINAL_LIST( '{0}' );";
            var uuid = await GetUserIndex(searchId);
            if (string.IsNullOrEmpty(uuid)) return nodata;
            var invoice = await Invoices(uuid, searchId);
            if (invoice == null || !invoice.Any()) return nodata;
            var mxCount = invoice.First().ItemCount.GetValueOrDefault();
            var command = string.Format(prc, searchId);
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<SearchFinalDto>(connection, command);
            if (response.Count() > mxCount)
                response = response.Take(mxCount);
            var translation = response.Select(x => TranslateTo<SearchFinalBo>(x));
            return translation;
        }

        private async Task<string?> GetUserIndex(string searchId)
        {
            const string prc = "CALL USP_GET_SEARCH_USER_INDEX( '{0}' );";
            var command = string.Format(prc, searchId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<UserIndexDto>(connection, command);
            return (response ?? new()).UserId;
        }

        public async Task<ActiveSearchOverviewBo?> GetActiveSearches(string searchId)
        {
            const string prc = "CALL USP_GET_ACTIVE_SEARCH_OVERVIEW( '{0}' );";
            var command = string.Format(prc, searchId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<ActiveSearchOverviewDto>(connection, command);
            if (response == null) return null;
            return ActiveSearchOverviewBo.FromDto(response);
        }

        public async Task<bool?> IsSearchPurchased(string searchId)
        {
            const string prc = "CALL USP_QUERY_INVOICE_IS_PAID( '{0}' );";
            var command = string.Format(prc, searchId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<SearchIsPaidDto>(connection, command);
            if (response == null) return null;
            return response.IsPaid;
        }

        public async Task<bool?> IsSearchDownloaded(string searchId)
        {
            const string prc = "CALL USP_QUERY_INVOICE_IS_DOWNLOADED( '{0}' );";
            var command = string.Format(prc, searchId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<SearchIsPaidDto>(connection, command);
            if (response == null) return null;
            return response.IsDownloaded;
        }
        public async Task<SearchIsPaidDto?> IsSearchPaidAndDownloaded(string searchId)
        {
            const string prc = "CALL USP_QUERY_INVOICE_IS_PAID_AND_DOWNLOADED( '{0}' );";
            var command = string.Format(prc, searchId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<SearchIsPaidDto>(connection, command);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> CreateOrUpdateDownloadRecord(string searchId, string? content = null)
        {
            try
            {
                const string prc = "CALL USP_GENERATE_DOWNLOAD_HISTORY_BY_SEARCH_INDEX( ?, ? );";
                const string prc_b = "CALL USP_SEARCH_DOWNLOAD_ROLLBACK_COMPLETED()";
                var parms = new DynamicParameters();
                parms.Add("search_index", searchId);
                parms.Add("excel_content", content);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                await _command.ExecuteAsync(connection, prc_b);
                return new KeyValuePair<bool, string>(true, "Command executed succesfully");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }
        public async Task<IEnumerable<ActiveSearchDetailBo>?> GetActiveSearchDetails(string userId)
        {
            const string prc = "CALL USP_GET_ACTIVE_SEARCH_PARAMETER_DETAILS( '{0}' );";
            var command = string.Format(prc, userId);
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<ActiveSearchDetailDto>(connection, command);
            if (response == null) return null;
            var translation = response.Select(x => TranslateTo<ActiveSearchDetailBo>(x));
            return translation;
        }

        public async Task<bool> CreateInvoice(string userId, string searchId)
        {
            var restriction = await GetSearchRestriction(userId);
            var maxRecords = GetAdjustedRecordCount(restriction);
            const string prc = "CALL USP_APPEND_SEARCH_INVOICE_HEADER( ?, ?, ? );";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("uu_index", userId);
                parameters.Add("search_index", searchId);
                parameters.Add("max_records", maxRecords);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<SearchInvoiceBo>> Invoices(string userId, string? searchId)
        {
            const string prc = "CALL USP_GET_SEARCH_INVOICE( ?, ? );";
            var parameters = new DynamicParameters();
            parameters.Add("uu_index", userId);
            parameters.Add("search_index", searchId);
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<SearchInvoiceDto>(connection, prc, parameters);
            var translation = response.Select(x => TranslateTo<SearchInvoiceBo>(x));
            return translation;
        }

        public async Task<KeyValuePair<bool, string>> Append(SearchTargetTypes search, string? id, object data, string? keyname = null)
        {
            try
            {
                var procedure = AppendProcs[search];
                var dapperParm = GetParameters(search, id, data, keyname);
                using (var connection = _context.CreateConnection())
                    await _command.ExecuteAsync(connection, procedure, dapperParm);
                return new KeyValuePair<bool, string>(true, "Command executed succesfully");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> Begin(string userId, string payload)
        {
            const string procedure = "CALL USP_APPEND_USER_SEARCH_AND_REQUEST ( ?, ?, ?, ? );";

            try
            {
                var searchGuid = Guid.NewGuid().ToString("D");
                var dapperParm = new DynamicParameters();
                dapperParm.Add("userRefId", userId);
                dapperParm.Add("searchGuid", searchGuid);
                dapperParm.Add("searchStart", DateTime.UtcNow);
                dapperParm.Add("jscontent", Encoding.UTF8.GetBytes(payload));
                using var connection = _context.CreateConnection();
                var inserted = await _command.QuerySingleOrDefaultAsync<SearchDto>(connection, procedure, dapperParm);
                if (inserted != null && !string.IsNullOrWhiteSpace(inserted.Id))
                {
                    return new KeyValuePair<bool, string>(true, inserted.Id);
                }
                return new KeyValuePair<bool, string>(false, "Insert new request failed to send response");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> Complete(string id)
        {
            const string procedure = "CALL USP_UPDATE_USER_SEARCH_COMPLETION(?, ?, ?);";

            try
            {
                int? noRecordCount = null;
                var dapperParm = new DynamicParameters();
                dapperParm.Add("searchUid", id);
                dapperParm.Add("estimatedRecords", noRecordCount);
                dapperParm.Add("completionDate", DateTime.UtcNow);
                using (var connection = _context.CreateConnection())
                    await _command.ExecuteAsync(connection, procedure, dapperParm);
                return new KeyValuePair<bool, string>(true, "Record update completed.");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateRowCount(string id, int rowCount)
        {
            const string procedure = "CALL USP_UPDATE_USER_SEARCH_COMPLETION(?, ?, ?);";

            try
            {
                DateTime? noDate = null;
                var dapperParm = new DynamicParameters();
                dapperParm.Add("searchUid", id);
                dapperParm.Add("estimatedRecords", rowCount);
                dapperParm.Add("completionDate", noDate);
                using (var connection = _context.CreateConnection())
                    await _command.ExecuteAsync(connection, procedure, dapperParm);
                return new KeyValuePair<bool, string>(true, "Record update completed.");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<IEnumerable<SearchTargetModel>?> GetTargets(SearchTargetTypes search, string? userId, string? id)
        {
            string prc = QueryProcs[search];
            var parms = new DynamicParameters();
            parms.Add("userUid", userId);
            parms.Add("searchUid", id);
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<SearchTargetDto>(connection, prc, parms);
            if (response == null) return null;
            var translation = response.Select(x => new SearchTargetModel
            {
                Component = x.Component,
                SearchId = x.SearchId,
                LineNbr = x.LineNbr,
                Line = x.Line,
                CreateDate = x.CreateDate
            });
            return translation;
        }

        public async Task<KeyValuePair<bool, object>> GetStaged(string id, string keyname)
        {
            const string procedure = "CALL USP_FIND_USER_SEARCH_STAGING(?, ?);";

            try
            {
                var dapperParm = new DynamicParameters();
                dapperParm.Add("searchItemId", id);
                dapperParm.Add("stagingName", keyname);
                using var connection = _context.CreateConnection();
                var staging = await _command.QueryAsync<SearchStagingDto>(connection, procedure, dapperParm);
                return new KeyValuePair<bool, object>(true, staging);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, object>(false, ex.Message);
            }
        }

        public async Task<bool> IsValidExternalId(string externalId)
        {
            const string prc = "CALL USP_QUERY_IS_VALID_EXTERNAL_INDEX( ? );";
            var parameters = new DynamicParameters();
            parameters.Add("external_index", externalId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<IsValidExternalIndexDto>(connection, prc, parameters);
            if (response == null) return false;
            return response.IsFound.GetValueOrDefault();
        }
        public async Task<PurchaseSummaryDto?> GetPurchaseSummary(string externalId)
        {
            const string prc = "CALL USP_GET_PURCHASE_DETAIL_BY_EXTERNAL_ID( ? );";
            var parameters = new DynamicParameters();
            parameters.Add("external_index", externalId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<PurchaseSummaryDto>(connection, prc, parameters);
            return response;
        }

        public async Task<bool> SetInvoicePurchaseDate(string externalId)
        {
            try
            {
                const string prc = "CALL USP_SET_INVOICE_PURCHASE_DATE( ? );";
                var procs = new[] { sprc, prc };
                var parameters = new DynamicParameters();
                parameters.Add("external_index", externalId);
                using var connection = _context.CreateConnection();
                for (var i = 0; i < procs.Length; i++)
                {
                    var current = procs[i];
                    if (i == 0)
                    {
                        await _command.ExecuteAsync(connection, current);
                        continue;
                    }
                    await _command.ExecuteAsync(connection, current, parameters);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<InvoiceDescriptionDto> InvoiceDescription(string id)
        {
            const string prc = "CALL USP_GET_INVOICE_DESCRIPTION( '{0}' );";
            var command = string.Format(prc, id);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<InvoiceDescriptionDto>(connection, command);
            return response ?? new();
        }


        public async Task<bool> UpdateSearchRowCount()
        {
            const string prc = "CALL USP_SEARCH_SET_FINAL_ROW_COUNT( );";
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> AppendPaymentSession(PaymentSessionDto dto)
        {
            const string prc = "CALL USP_INSERT_PAYMENT_SESSION( ?, ?, ?, ?, ?, ?, ?, ? );";
            try
            {
                using var connection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("idx", dto.Id);
                parameters.Add("user_id", dto.UserId);
                parameters.Add("session_type", dto.SessionType);
                parameters.Add("session_id", dto.SessionId);
                parameters.Add("intent_id", dto.IntentId);
                parameters.Add("client_id", dto.ClientId);
                parameters.Add("external_id", dto.ExternalId);
                parameters.Add("js_text", dto.JsText);
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<PaymentSessionDto?> GetPaymentSession(string externalId)
        {
            const string prc = "CALL USP_GET_PAYMENT_SESSION( ? );";
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("external_id", externalId);
            var response = await _command.QuerySingleOrDefaultAsync<PaymentSessionDto>(connection, prc, parameters);
            return response;
        }

        public async Task<IEnumerable<PurchasedSearchBo>?> GetPurchases(string userId)
        {
            // note: you need unit test coverage for this method
            const string prc = "CALL USP_QUERY_USER_PURCHASE_HISTORY( '{0}' );";
            var command = string.Format(prc, userId);
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<PurchasedSearchDto>(connection, command);
            if (response == null) return null;
            var translation = response.Select(x => TranslateTo<PurchasedSearchBo>(x));
            return translation;
        }

        public async Task<DownloadHistoryDto?> AllowDownloadRollback(string userId, string searchId)
        {
            const string prc = "CALL USP_SEARCH_DOWNLOAD_ROLLBACK( ?, ? );";
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("search_index", searchId);
            parameters.Add("user_index", userId);
            var response = await _command.QuerySingleOrDefaultAsync<DownloadHistoryDto>(connection, prc, parameters);
            return response;
        }

        public async Task<bool> AppendAdHocSession(AdHocSessionBo dto)
        {
            try
            {
                const string prc = "CALL USP_INSERT_ADHOC_SESSION( ?, ?, ?, ? );";
                using IDbConnection connection = _context.CreateConnection();
                DynamicParameters parms = new();
                parms.Add("user_id", dto.UserId);
                parms.Add("intent_id", dto.IntentId);
                parms.Add("client_id", dto.ClientId);
                parms.Add("external_id", dto.ExternalId);
                await _command.ExecuteAsync(connection, prc, parms);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> CompleteAdHocSession(string externalId)
        {
            try
            {
                const string prc = "CALL USP_COMPLETE_ADHOC_SESSION( ? );";
                using IDbConnection connection = _context.CreateConnection();
                DynamicParameters parms = new();
                parms.Add("external_id", externalId);
                await _command.ExecuteAsync(connection, prc, parms);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<AdHocSessionBo?> FindAdHocSession(string externalId)
        {
            try
            {
                const string prc = "CALL USP_GET_ADHOC_SESSION( ? );";
                using IDbConnection connection = _context.CreateConnection();
                DynamicParameters parms = new();
                parms.Add("external_id", externalId);
                var dto = await _command.QuerySingleOrDefaultAsync<AdHocSessionDto>(connection, prc, parms);
                if (dto == null) return null;
                return TranslateTo<AdHocSessionBo>(dto);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> FlagError(string searchId)
        {
            const string prc = "CALL USP_SEARCH_SET_STATUS_TO_ERROR( '{0}' );";
            try
            {
                var command = string.Format(prc, searchId);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, command);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static int GetAdjustedRecordCount(SearchRestrictionDto? dto)
        {
            const int count = 100000;
            if (dto == null) return count;
            var mxpermonth = dto.MaxPerMonth.GetValueOrDefault(count) - dto.ThisMonth.GetValueOrDefault();
            var mxperyear = dto.MaxPerYear.GetValueOrDefault(count) - dto.ThisYear.GetValueOrDefault();
            return Math.Min(mxpermonth, mxperyear);
        }

        private static DynamicParameters GetParameters(SearchTargetTypes search, string? id, object data, string? keyname = null)
        {

            var dapperParm = new DynamicParameters();
            if (search == SearchTargetTypes.Staging)
            {
                dapperParm.Add("searchItemId", id);
                dapperParm.Add("stagingName", keyname);
                dapperParm.Add("jscontent", data);
                var isByte = data is byte[];
                dapperParm.Add("isByteArray", Convert.ToByte(isByte ? 1 : 0));
            }
            else
            {
                dapperParm.Add("searchItemId", id);
                dapperParm.Add("jscontent", data);

            }
            return dapperParm;
        }
        private static T TranslateTo<T>(object source) where T : class, new()
        {

            var tmp = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(tmp) ?? new();
        }

        private const string sprc = "CALL USP_INVOICE_SET_EXTERNALID_AND_PAYMENT_DATE()";
        private static readonly Dictionary<SearchTargetTypes, string> AppendProcs = new(){
            { SearchTargetTypes.Detail, "CALL USP_APPEND_USER_SEARCH_DETAIL( ?, ? );" },
            { SearchTargetTypes.Request, "CALL USP_APPEND_USER_SEARCH_REQUEST( ?, ? );" },
            { SearchTargetTypes.Response, "CALL USP_APPEND_USER_SEARCH_RESPONSE( ?, ? );" },
            { SearchTargetTypes.Status, "CALL USP_APPEND_USER_SEARCH_STATUS( ?, ? );" },
            { SearchTargetTypes.Staging, "CALL USP_APPEND_USER_SEARCH_STAGING( ?, ?, ?, ? );" },
            };

        private static readonly Dictionary<SearchTargetTypes, string> QueryProcs = new(){
            { SearchTargetTypes.Detail, "CALL USP_QUERY_USER_SEARCH_DETAIL( ?, ? );" },
            { SearchTargetTypes.Request, "CALL USP_QUERY_USER_SEARCH_REQUEST( ?, ? );" },
            { SearchTargetTypes.Response, "CALL USP_QUERY_USER_SEARCH_RESPONSE( ?, ? );" },
            { SearchTargetTypes.Status, "CALL USP_QUERY_USER_SEARCH_STATUS( ?, ? );" },
            { SearchTargetTypes.Staging, "CALL USP_QUERY_USER_SEARCH_STAGING( ?, ? );" },
            };
    }
}

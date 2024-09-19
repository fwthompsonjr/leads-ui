using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.enumerations;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;

namespace legallead.jdbc.implementations
{
    public class QueueWorkRepository : BaseRepository<QueueWorkingDto>, IQueueWorkRepository
    {
        public QueueWorkRepository(DataContext context) : base(context)
        {
        }

        public List<QueueWorkingBo> InsertRange(string json)
        {
            try
            {
                var prc = Procs[ProcedureNames.Insert];
                var parms = new DynamicParameters();
                parms.Add("jspayload", json);
                using var connection = _context.CreateConnection();
                var list = _command.QueryAsync<QueueWorkingDto>(connection, prc, parms).GetAwaiter().GetResult();
                if (list == null || !list.Any()) return new();
                return TranslateTo<List<QueueWorkingBo>>(list);
            }
            catch (Exception)
            {
                return new();
            }
        }

        public QueueWorkingBo? UpdateStatus(QueueWorkingBo updateBo)
        {
            try
            {
                var prc = Procs[ProcedureNames.Update];
                var parms = new DynamicParameters();
                var obj = new
                {
                    updateBo.Id,
                    updateBo.SearchId,
                    updateBo.Message,
                    updateBo.StatusId
                };
                var json = JsonConvert.SerializeObject(obj);
                parms.Add("jspayload", json);
                using var connection = _context.CreateConnection();
                var list = _command.QueryAsync<QueueWorkingDto>(connection, prc, parms).GetAwaiter().GetResult();
                if (list == null || !list.Any()) return null;
                return TranslateTo<QueueWorkingBo>(list.First());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public QueuePersonDataBo? UpdatePersonData(QueuePersonDataBo bo)
        {
            try
            {
                const string prc = "CALL USP_QUEUE_STAGE_PERSON_DATA( ?, ? );";
                var parms = new DynamicParameters();
                parms.Add("indx", bo.Id);
                parms.Add("data_bin", bo.Data);
                using var connection = _context.CreateConnection();
                _command.ExecuteAsync(connection, prc, parms).GetAwaiter().GetResult();
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<QueueWorkingBo> Fetch()
        {
            try
            {
                var prc = Procs[ProcedureNames.Fetch];
                using var connection = _context.CreateConnection();
                var list = _command.QueryAsync<QueueWorkingDto>(connection, prc).GetAwaiter().GetResult();
                if (list == null || !list.Any()) return new();
                return TranslateTo<List<QueueWorkingBo>>(list);
            }
            catch (Exception)
            {
                return new();
            }
        }

        public QueueWorkingUserBo? GetUserBySearchId(string? searchId)
        {
            if (string.IsNullOrEmpty(searchId)) return null;
            if (!Guid.TryParse(searchId, out var _)) return null;
            try
            {
                var prc = Procs[ProcedureNames.Insert];
                var parms = new DynamicParameters();
                parms.Add("search_index", searchId);
                using var connection = _context.CreateConnection();
                var list = _command.QueryAsync<CustomerDto>(connection, prc, parms).GetAwaiter().GetResult();
                if (list == null || !list.Any()) return new();
                return TranslateTo<QueueWorkingUserBo>(list.First());
            }
            catch (Exception)
            {
                return new();
            }
        }

        public async Task<List<StatusSummaryByCountyBo>> GetSummary(QueueStatusTypes statusType)
        {
            try
            {
                var prc = statusProcs[statusType];
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<StatusSummaryDto>(connection, prc);
                if (response == null || !response.Any()) return new();
                return TranslateTo<List<StatusSummaryByCountyBo>>(response);
            }
            catch (Exception)
            {
                return new();
            }
        }

        public async Task<List<StatusSummaryBo>> GetStatus()
        {
            try
            {
                var prc = Procs[ProcedureNames.GetStatus];
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<StatusDto>(connection, prc);
                if (response == null || !response.Any()) return new();
                return TranslateTo<List<StatusSummaryBo>>(response);
            }
            catch (Exception)
            {
                return new();
            }
        }

        public async Task<List<QueueNonPersonBo>> GetNonPersonData()
        {
            try
            {
                var prc = Procs[ProcedureNames.GetNonPerson];
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<QueueNonPersonDto>(connection, prc);
                if (response == null || !response.Any()) return new();
                return TranslateTo<List<QueueNonPersonBo>>(response);
            }
            catch (Exception)
            {
                return new();
            }
        }

        private static T TranslateTo<T>(object source) where T : class, new()
        {

            var tmp = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(tmp) ?? new();
        }

        private static readonly Dictionary<ProcedureNames, string> Procs = new(){
            { ProcedureNames.Insert, "CALL USP_INSERT_QUEUEWORKING( ? );" },
            { ProcedureNames.Update, "CALL USP_UPDATE_QUEUEWORKING( ? );" },
            { ProcedureNames.Fetch, "CALL USP_FETCH_QUEUEWORKING();" },
            { ProcedureNames.GetUser, "CALL USP_FETCH_ACCOUNT_BY_SEARCH_INDEX( ? );" },
            { ProcedureNames.GetStatus, "CALL USP_QUEUE_GET_PROGRESS_SUMMARY_YTD();" },
            { ProcedureNames.GetNonPerson, "CALL USP_QUEUE_GET_NON_PERSON_DATA();" }
            };

        private enum ProcedureNames
        {
            Insert = 0,
            Update = 1,
            Fetch = 2,
            GetUser = 3,
            GetStatus = 4,
            GetNonPerson = 5,
        }


        private static readonly Dictionary<QueueStatusTypes, string> statusProcs = new()
        {
            { QueueStatusTypes.Error, "CALL USP_QUEUE_GET_ERRORS_BY_COUNTY();" },
            { QueueStatusTypes.Submitted, "CALL USP_QUEUE_GET_WAITING_BY_COUNTY();" },
            { QueueStatusTypes.Purchased, "CALL USP_QUEUE_GET_PURCHASED_BY_COUNTY();" }
        };
    }
}
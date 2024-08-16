using Dapper;
using legallead.jdbc.entities;
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

        private static T TranslateTo<T>(object source) where T : class, new()
        {

            var tmp = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(tmp) ?? new();
        }

        private static readonly Dictionary<ProcedureNames, string> Procs = new(){
            { ProcedureNames.Insert, "CALL USP_INSERT_QUEUEWORKING( ? );" },
            { ProcedureNames.Update, "CALL USP_UPDATE_QUEUEWORKING( ? );" },
            { ProcedureNames.Fetch, "CALL USP_FETCH_QUEUEWORKING();" },
            };

        private enum ProcedureNames
        {
            Insert = 0,
            Update = 1,
            Fetch = 2
        }
    }
}
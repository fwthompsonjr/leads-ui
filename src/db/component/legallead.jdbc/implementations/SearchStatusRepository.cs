using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;

namespace legallead.jdbc.implementations
{
    public class SearchStatusRepository : BaseRepository<SearchDto>, ISearchStatusRepository
    {
        public SearchStatusRepository(DataContext context) : base(context)
        {
        }

        public bool Begin(WorkBeginningBo bo)
        {
            try
            {
                var prc = Procs[ProcedureNames.Begin];
                var parm = new DynamicParameters();
                parm.Add("js", JsonConvert.SerializeObject(bo));
                using var connection = _context.CreateConnection();
                _command.ExecuteAsync(connection, prc, parm).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(WorkStatusBo bo)
        {
            try
            {
                var prc = Procs[ProcedureNames.Status];
                var parm = new DynamicParameters();
                parm.Add("js", JsonConvert.SerializeObject(bo));
                using var connection = _context.CreateConnection();
                _command.ExecuteAsync(connection, prc, parm).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IEnumerable<WorkingSearchBo>? List()
        {
            try
            {
                var prc = Procs[ProcedureNames.All];
                using var connection = _context.CreateConnection();
                var list = _command.QueryAsync<WorkingSearchDto>(connection, prc).GetAwaiter().GetResult();
                if (list == null) return null;
                return TranslateTo<List<WorkingSearchBo>>(list);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static T TranslateTo<T>(object source) where T : class, new()
        {

            var tmp = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(tmp) ?? new();
        }

        private static readonly Dictionary<ProcedureNames, string> Procs = new(){
            { ProcedureNames.All, "CALL USP_WORKTABLE_ALL();" },
            { ProcedureNames.Begin, "CALL USP_WORKTABLE_BEGIN( ? );" },
            { ProcedureNames.Status, "CALL USP_WORKTABLE_STATUS( ? );" },
            };

        private enum ProcedureNames
        {
            All = 0,
            Begin = 1,
            Status = 2
        }
    }
}
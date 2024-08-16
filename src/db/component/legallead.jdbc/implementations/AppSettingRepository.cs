using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;

namespace legallead.jdbc.implementations
{
    public class AppSettingRepository : BaseRepository<AppSettingDto>, IAppSettingRepository
    {
        public AppSettingRepository(DataContext context) : base(context)
        {
        }

        public AppSettingBo? FindKey(string keyName)
        {
            try
            {
                var prc = Procs[ProcedureNames.Find];
                var parms = new DynamicParameters();
                parms.Add("key_name", keyName);
                using var connection = _context.CreateConnection();
                var list = _command.QueryAsync<AppSettingDto>(connection, prc, parms).GetAwaiter().GetResult();
                if (list == null || !list.Any()) return null;
                return TranslateTo<AppSettingBo>(list.First());
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
            { ProcedureNames.Find, "CALL USP_GET_APPSETTING( ? );" },
            };

        private enum ProcedureNames
        {
            Find = 0
        }
    }
}
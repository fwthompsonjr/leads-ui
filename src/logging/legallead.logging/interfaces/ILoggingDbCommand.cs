using Dapper;
using legallead.logging.entities;
using System.Data;

namespace legallead.logging.interfaces
{
    public interface ILoggingDbCommand
    {
        Task ExecuteAsync(IDbConnection conn, string sql, DynamicParameters? arg = null);

        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : CommonBaseDto, new();

        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : CommonBaseDto, new();
    }
}
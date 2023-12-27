using Dapper;
using legallead.jdbc.entities;
using System.Data;

namespace legallead.jdbc.interfaces
{
    public interface IDapperCommand
    {
        Task ExecuteAsync(IDbConnection conn, string sql, DynamicParameters? arg = null);

        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : BaseDto, new();

        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : BaseDto, new();
    }
}
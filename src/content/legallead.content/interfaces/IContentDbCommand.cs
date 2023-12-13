using Dapper;
using legallead.content.entities;
using System.Data;

namespace legallead.content.interfaces
{
    public interface IContentDbCommand
    {
        Task ExecuteAsync(IDbConnection conn, string sql, DynamicParameters? arg = null);

        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : CommonBaseDto, new();

        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : CommonBaseDto, new();
    }
}
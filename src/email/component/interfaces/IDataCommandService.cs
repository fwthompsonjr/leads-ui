using Dapper;
using legallead.email.entities;
using System.Data;

namespace legallead.email.services
{
    internal interface IDataCommandService
    {
        Task ExecuteAsync(IDbConnection conn, string sql, DynamicParameters? arg = null);
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null) where T : BaseDto, new();
        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null) where T : BaseDto, new();
    }
}
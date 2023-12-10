using Dapper;
using legallead.jdbc.entities;
using System.Data;

namespace legallead.jdbc.interfaces
{
    public interface IDapperCommand
    {


        Task ExecuteAsync(IDbConnection conn, string sql, params object[] args);

        Task ExecuteAsync(IDbConnection conn, string sql, DynamicParameters arg);

        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, DynamicParameters arg)
            where T : BaseDto, new();

        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, params object[] args)
            where T : BaseDto, new();

        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, params object[] args)
            where T : BaseDto, new();

        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, Dapper.DynamicParameters arg)
            where T : BaseDto, new();
    }
}
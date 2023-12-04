using Dapper;
using legallead.jdbc.interfaces;
using System.Data;

namespace legallead.jdbc.helpers
{
    public class DapperExecutor : IDapperCommand
    {
        public async Task ExecuteAsync(IDbConnection conn, string sql, params object[] args)
        {
            await conn.ExecuteAsync(sql);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, params object[] args)
        {
            var response = await conn.QueryAsync<T>(sql);
            return response;
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, DynamicParameters arg)
        {
            var response = await conn.QuerySingleOrDefaultAsync<T>(sql, arg);
            return response;
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, params object[] args)
        {
            var response = await conn.QuerySingleOrDefaultAsync<T>(sql);
            return response;
        }
    }
}
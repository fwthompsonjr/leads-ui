using System.Data;

namespace legallead.jdbc.interfaces
{
    public interface IDapperCommand
    {
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, params object[] args);

        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, params object[] args);

        Task ExecuteAsync(IDbConnection conn, string sql, params object[] args);
    }
}
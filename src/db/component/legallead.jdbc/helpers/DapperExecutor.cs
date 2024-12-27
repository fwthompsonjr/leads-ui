using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace legallead.jdbc.helpers
{
    public class DapperExecutor : IDapperCommand
    {
        public async Task ExecuteAsync(IDbConnection conn, string sql, DynamicParameters? arg = null)
        {
            await Task.Run(() =>
            {
                Execute(conn, sql, arg);
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Test coverage through integration tests")]
        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : BaseDto, new()
        {
            return await Task.Run(() =>
            {
                return GetResult<T>(conn, sql, arg);
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Test coverage through integration tests")]
        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : BaseDto, new()
        {
            var response = await Task.Run(() =>
            {
                return GetResult<T>(conn, sql, arg);
            });
            if (!response.Any()) return default;
            return response.FirstOrDefault();
        }

        [ExcludeFromCodeCoverage(Justification = "Private static testing from parent method.")]
        private static List<T> GetResult<T>(IDbConnection conn, string sql, DynamicParameters? arg = null) where T : BaseDto, new()
        {
            try
            {
                var response = new List<T>();
                var cmmd = CreateCommand(conn, sql, arg);
                var reader = cmmd.ExecuteReader();
                while (reader.Read())
                {
                    var record = new T();
                    var columnCount = reader.FieldCount;
                    for (int index = 0; index < columnCount; index++)
                    {
                        var name = reader.GetName(index);
                        record[name] = GetFieldValue(reader, index);
                    }
                    response.Add(record);
                }
                return response;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }


        [ExcludeFromCodeCoverage(Justification = "Private static testing from parent method.")]
        private static void Execute(IDbConnection conn, string sql, DynamicParameters? arg = null)
        {
            try
            {
                var cmmd = CreateCommand(conn, sql, arg);
                cmmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }


        [ExcludeFromCodeCoverage(Justification = "Private static testing from parent method.")]
        private static IDbCommand CreateCommand(IDbConnection conn, string sql, DynamicParameters? arg = null)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            var cmmd = conn.CreateCommand();
            cmmd.CommandText = sql;
            cmmd.CommandTimeout = 30;
            arg?.ParameterNames.ToList().ForEach(parameter =>
            {
                var p = cmmd.CreateParameter();
                p.ParameterName = parameter;
                p.Value = arg.Get<object?>(parameter);
                cmmd.Parameters.Add(p);
            });
            return cmmd;
        }

        [ExcludeFromCodeCoverage(Justification = "Private static testing from parent method.")]
        private static object? GetFieldValue(IDataReader reader, int index)
        {
            try
            {
                var data = reader.GetValue(index);
                if (data is not Guid guid) return data;
                return guid.ToString("D");
            } 
            catch
            {
                return null;
            }
        }
    }
}
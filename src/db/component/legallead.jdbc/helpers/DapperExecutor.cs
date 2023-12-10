using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using System.Data;
using System.Data.Common;

namespace legallead.jdbc.helpers
{
    public class DapperExecutor : IDapperCommand
    {
        public async Task ExecuteAsync(IDbConnection conn, string sql, params object[] args)
        {
            await conn.ExecuteAsync(sql);
        }

        public async Task ExecuteAsync(IDbConnection conn, string sql, DynamicParameters arg)
        {
            await conn.ExecuteAsync(sql, arg);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, params object[] args)
            where T : BaseDto, new()
        {
            return await Task.Run(() =>
            {
                return GetResult<T>(conn, sql);
            });
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, DynamicParameters arg)
            where T : BaseDto, new()
        {
            return await Task.Run(() =>
            {
                return GetResult<T>(conn, sql, arg);
            });
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, DynamicParameters arg)
            where T : BaseDto, new()
        {
            var response = await Task.Run(() =>
            {
                return GetResult<T>(conn, sql, arg);
            });
            if (!response.Any()) return default;
            return response.FirstOrDefault();
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, params object[] args)
            where T : BaseDto, new()
        {
            var response = await Task.Run(() =>
            {
                return GetResult<T>(conn, sql);
            });
            if (!response.Any()) return default;
            return response.FirstOrDefault();
        }


        private static List<T> GetResult<T>(IDbConnection conn, string sql) where T : BaseDto, new()
        {
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                var response = new List<T>();
                var cmmd = conn.CreateCommand();
                cmmd.CommandText = sql;
                var reader = cmmd.ExecuteReader();
                while (reader.Read())
                {
                    var record = new T();
                    var columnCount = reader.FieldCount;
                    for (int index = 0; index < columnCount; index++)
                    {
                        var name = reader.GetName(index);
                        record[name] = reader[index];
                    }
                    response.Add(record);
                }
                return response;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
                conn.Dispose();
            }
        }
        private static List<T> GetResult<T>(IDbConnection conn, string sql, DynamicParameters arg) where T : BaseDto, new()
        {
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                var response = new List<T>();
                var cmmd = conn.CreateCommand();
                cmmd.CommandText = sql;
                arg.ParameterNames.ToList().ForEach(parameter =>
                {
                    var current = arg.Get<object>(parameter);
                    cmmd.Parameters[parameter] = current;
                });
                var reader = cmmd.ExecuteReader();
                while (reader.Read())
                {
                    var record = new T();
                    var columnCount = reader.FieldCount;
                    for (int index = 0; index < columnCount; index++)
                    {
                        var name = reader.GetName(index);
                        record[name] = reader[index];
                    }
                    response.Add(record);
                }
                return response;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
                conn.Dispose();
            }
        }


    }
}
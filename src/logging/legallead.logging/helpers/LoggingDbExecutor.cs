﻿using Dapper;
using legallead.logging.entities;
using legallead.logging.interfaces;
using System.Data;

namespace legallead.logging.helpers
{
    public class LoggingDbExecutor : ILoggingDbCommand
    {
        public async Task ExecuteAsync(IDbConnection conn, string sql, DynamicParameters? arg = null)
        {
            await Task.Run(() =>
            {
                Execute(conn, sql, arg);
            });
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : CommonBaseDto, new()
        {
            return await Task.Run(() =>
            {
                return GetResult<T>(conn, sql, arg);
            });
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, DynamicParameters? arg = null)
            where T : CommonBaseDto, new()
        {
            var response = await Task.Run(() =>
            {
                return GetResult<T>(conn, sql, arg);
            });
            if (!response.Any()) return default;
            return response.FirstOrDefault();
        }

        private static List<T> GetResult<T>(IDbConnection conn, string sql, DynamicParameters? arg = null) where T : CommonBaseDto, new()
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
                        record[name] = reader[index];
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

        private static IDbCommand CreateCommand(IDbConnection conn, string sql, DynamicParameters? arg = null)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            var cmmd = conn.CreateCommand();
            cmmd.CommandText = sql;
            arg?.ParameterNames.ToList().ForEach(parameter =>
            {
                var p = cmmd.CreateParameter();
                p.ParameterName = parameter;
                p.Value = arg.Get<object?>(parameter);
                cmmd.Parameters.Add(p);
            });
            return cmmd;
        }
    }
}
﻿using legallead.jdbc.helpers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace legallead.search.api.Health
{
    [ExcludeFromCodeCoverage]
    public class DbConnectionHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider _provider;

        public DbConnectionHealthCheck(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await Task.Run(() =>
                {
                    var db = _provider.GetService<DataContext>();
                    if (db == null) return HealthCheckResult.Unhealthy("Unable to initialize data context object");
                    using var connection = db.CreateConnection();
                    _ = GetScalar(connection, "SELECT 1");
                    return HealthCheckResult.Healthy("Database connection opened successfully.");
                });
                return response;
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    description: "Create DbConnection Context",
                    exception: ex);
            }
        }

        private static object? GetScalar(IDbConnection conn, string sql)
        {
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                var cmmd = conn.CreateCommand();
                cmmd.CommandText = sql;
                return cmmd.ExecuteScalar();
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}
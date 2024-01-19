using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class BgComponentRepository : BaseRepository<BackgroundComponentDto>, IBgComponentRepository
    {
        public BgComponentRepository(DataContext context) : base(context)
        {
        }
        public async Task<bool> GetStatus(string? componentName, string? serviceName)
        {
            if (string.IsNullOrWhiteSpace(componentName) &&
                string.IsNullOrWhiteSpace(serviceName)) return false;

            const string prc = "CALL USP_COMPONENT_GET_STATUS( ?, ? );";
            var dapperParm = new DynamicParameters();
            dapperParm.Add("component_name", componentName);
            dapperParm.Add("service_name", serviceName);
            using var connection = _context.CreateConnection();
            var response = (await _command.QueryAsync<BackgroundComponentStatusDto>(
                connection, prc, dapperParm)).ToList();
            if (!response.Any()) return true;
            return response[0].StatusId.GetValueOrDefault() == 1;
        }

        public async Task<bool> ReportHealth(string? componentName, string? serviceName, string health)
        {
            try
            {
                const string procedure = "CALL USP_COMPONENT_REPORT_HEALTH ( ?, ?, ?);";
                var dapperParm = new DynamicParameters();
                dapperParm.Add("component_name", componentName);
                dapperParm.Add("service_name", serviceName);
                dapperParm.Add("health_name", health);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, procedure, dapperParm);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SetStatus(string? componentName, string? serviceName, string status)
        {
            try
            {
                const string procedure = "CALL USP_COMPONENT_SET_STATUS ( ?, ?, ?);";
                var dapperParm = new DynamicParameters();
                dapperParm.Add("component_name", componentName);
                dapperParm.Add("service_name", serviceName);
                dapperParm.Add("status_name", status);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, procedure, dapperParm);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
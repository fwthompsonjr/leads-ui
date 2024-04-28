using legallead.email.entities;
using legallead.email.interfaces;
using legallead.email.models;
using Newtonsoft.Json;

namespace legallead.email.services
{
    internal class UserSettingInfrastructure(
        IDataCommandService commandService,
        IDataConnectionService connection) : IUserSettingInfrastructure
    {
        private readonly IDataCommandService _db = commandService;
        private readonly IDataConnectionService _connection = connection;
        public async Task<List<UserEmailSettingBo>?> GetSettings(UserSettingQuery query)
        {
            if (!query.IsValid) { return null; }
            var db = _connection.CreateConnection();
            var parms = query.GetParameters();
            var sql = CommandText();
            var response = await _db.QueryAsync<UserEmailSettingDto>(db, sql, parms);
            var json = JsonConvert.SerializeObject(response);
            return JsonConvert.DeserializeObject<List<UserEmailSettingBo>>(json) ?? [];
        }
        private static string? _commandText;
        private static string CommandText()
        {
            if (!string.IsNullOrEmpty(_commandText)) { return _commandText; }
            var dto = new UserEmailSettingDto();
            _commandText = $"CALL {dto.TableName} ( ?, ? )";
            return _commandText;
        }

    }
}

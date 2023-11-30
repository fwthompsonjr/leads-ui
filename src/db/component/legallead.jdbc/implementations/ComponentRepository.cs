using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class ComponentRepository : IComponentRepository
    {
        private const string tableName = "applications";
        private readonly DataContext _context;
        private readonly IDapperCommand _command;

        public ComponentRepository(DataContext context)
        {
            _context = context;
            _command = context.GetCommand;
            Task.Run(() =>
            {
                context.Init().ConfigureAwait(false);
            });
        }

        public async Task<IEnumerable<Component>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName};";
            return await _command.QueryAsync<Component>(connection, sql);
        }

        public async Task<Component?> GetById(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE Id = '@Id';".Replace("@Id", id);
            return await _command.QuerySingleOrDefaultAsync<Component>(connection, sql);
        }

        public async Task<Component?> GetByName(string name)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE Name = '@name';".Replace("@name", name);
            return await _command.QuerySingleOrDefaultAsync<Component>(connection, sql);
        }

        public async Task Create(Component application)
        {
            using var connection = _context.CreateConnection();
            var sql = $"INSERT INTO {tableName} (Id, Name) VALUES ( '@Id', '@Name');"
                .Replace("@Id", application.Id)
                .Replace("@Name", application.Name);
            await _command.ExecuteAsync(connection, sql);
        }

        public async Task Update(Component application)
        {
            using var connection = _context.CreateConnection();
            var sql = $"UPDATE {tableName} SET Name = '@Name' WHERE Id = '@Id';"
                .Replace("@Id", application.Id)
                .Replace("@Name", application.Name);
            await _command.ExecuteAsync(connection, sql);
        }

        public async Task Delete(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"DELETE FROM {tableName} WHERE Id = '@Id';".Replace("@Id", id); ;
            await _command.ExecuteAsync(connection, sql);
        }
    }
}
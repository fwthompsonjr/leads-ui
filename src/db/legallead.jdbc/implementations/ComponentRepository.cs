using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    internal class ComponentRepository : IComponentRepository
    {
        private const string tableName = "applications";
        private readonly DataContext _context;

        public ComponentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Component>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName};";
            return await connection.QueryAsync<Component>(sql);
        }

        public async Task<Component?> GetById(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE Id = @id;";
            return await connection.QuerySingleOrDefaultAsync<Component>(sql, new { id });
        }

        public async Task<Component?> GetByName(string name)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE Name = @name;";
            return await connection.QuerySingleOrDefaultAsync<Component>(sql, new { name });
        }

        public async Task Create(Component application)
        {
            using var connection = _context.CreateConnection();
            var sql = $"INSERT INTO {tableName} (Id, Name) VALUES (@Id, @Name);";
            await connection.ExecuteAsync(sql, application);
        }

        public async Task Update(Component application)
        {
            using var connection = _context.CreateConnection();
            var sql = $"UPDATE {tableName} SET Name = @Name WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, application);
        }

        public async Task Delete(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"DELETE FROM {tableName} WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, new { id });
        }
    }
}

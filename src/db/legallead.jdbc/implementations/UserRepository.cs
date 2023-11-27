using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    internal class UserRepository : IUserRepository
    {
        private const string tableName = "users";
        private readonly DataContext _context;
        private readonly IDapperCommand _command;

        public UserRepository(DataContext context)
        {
            _context = context;
            _command = context.GetCommand;
        }
        public async Task Create(User user)
        {
            using var connection = _context.CreateConnection();
            var sql = $"INSERT INTO {tableName} (Id, UserName, Email, PasswordHash, PasswordSalt) " +
                "VALUES (@Id, @UserName, @Email, @PasswordHash, @PasswordSalt);";
            await _command.ExecuteAsync(connection, sql, user);
        }

        public async Task Delete(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"DELETE FROM {tableName} WHERE Id = @Id;";
            await _command.ExecuteAsync(connection, sql, new { id });
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName};";
            return await _command.QueryAsync<User>(connection, sql);
        }

        public async Task<User?> GetByEmail(string email)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE Email = @email;";
            return await _command.QuerySingleOrDefaultAsync<User>(connection, sql, new { email });
        }

        public async Task<User?> GetById(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE Id = @id;";
            return await _command.QuerySingleOrDefaultAsync<User>(connection, sql, new { id });
        }

        public async Task<User?> GetByName(string name)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE UserName = @UserName;";
            return await _command.QuerySingleOrDefaultAsync<User>(connection, sql, new { name });
        }

        public async Task Update(User user)
        {
            using var connection = _context.CreateConnection();
            var sql = $"UPDATE {tableName} " +
                "SET UserName = @UserName " +
                " Email = @Email " +
                " PasswordHash = @PasswordHash " +
                " PasswordSalt = @PasswordSalt " +
                "WHERE Id = @Id;";
            await _command.ExecuteAsync(connection, sql, user);
        }
    }
}

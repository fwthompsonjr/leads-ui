using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserRepository : IUserRepository
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
                "VALUES ('@Id', '@UserName', '@Email', '@PasswordHash', '@PasswordSalt');";
            sql = sql.Replace("@Id", user.Id);
            sql = sql.Replace("@UserName", user.UserName);
            sql = sql.Replace("@Email", user.Email);
            sql = sql.Replace("@PasswordHash", user.PasswordHash);
            sql = sql.Replace("@PasswordSalt", user.PasswordSalt);
            await _command.ExecuteAsync(connection, sql, user);
        }

        public async Task Delete(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"DELETE FROM {tableName} WHERE Id = '@Id';";
            sql = sql.Replace("@Id", id);
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
            var sql = $"SELECT * FROM {tableName} WHERE Email = '@email' ORDER BY Email LIMIT 1;";
            sql = sql.Replace("@email", email);
            return await _command.QuerySingleOrDefaultAsync<User>(connection, sql, new { email });
        }

        public async Task<User?> GetById(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE Id = '@id' ORDER BY Id LIMIT 1;";
            sql = sql.Replace("@id", id);
            return await _command.QuerySingleOrDefaultAsync<User>(connection, sql, new { id });
        }

        public async Task<User?> GetByName(string name)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {tableName} WHERE UserName = '@UserName' ORDER BY UserName LIMIT 1;";
            sql = sql.Replace("@UserName", name);
            return await _command.QuerySingleOrDefaultAsync<User>(connection, sql, new { name });
        }

        public async Task Update(User user)
        {
            using var connection = _context.CreateConnection();
            var sql = $"UPDATE {tableName} " +
                "SET UserName = '@UserName' " +
                " Email = '@Email' " +
                " PasswordHash = '@PasswordHash' " +
                " PasswordSalt = '@PasswordSalt' " +
                "WHERE Id = '@Id';";
            sql = sql.Replace("@Id", user.Id);
            sql = sql.Replace("@UserName", user.UserName);
            sql = sql.Replace("@Email", user.Email);
            sql = sql.Replace("@PasswordHash", user.PasswordHash);
            sql = sql.Replace("@PasswordSalt", user.PasswordSalt);
            await _command.ExecuteAsync(connection, sql, user);
        }
    }
}

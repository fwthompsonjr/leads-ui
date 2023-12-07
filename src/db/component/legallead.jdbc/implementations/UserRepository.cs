using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;

namespace legallead.jdbc.implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmail(string email)
        {
            using var connection = _context.CreateConnection();
            var parm = new User { Email = email };
            var sql = _sut.SelectSQL(parm).Replace(";", " LIMIT 1;");
            var parms = _sut.SelectParameters(parm);
            return await _command.QuerySingleOrDefaultAsync<User>(connection, sql, parms);
        }

        public async Task<User?> GetByName(string name)
        {
            using var connection = _context.CreateConnection();
            var parm = new User { UserName = name };
            var sql = _sut.SelectSQL(parm).Replace(";", " LIMIT 1;");
            var parms = _sut.SelectParameters(parm);
            return await _command.QuerySingleOrDefaultAsync<User>(connection, sql, parms);
        }

        public async Task<KeyValuePair<bool, User?>> IsValidUserAsync(UserModel model)
        {
            var response = new KeyValuePair<bool, User?>(false, null);
            var findEmail = await GetByEmail(model.Email);
            var findName = await GetByName(model.UserName);
            if (findEmail == null && findName == null) return response;
            if (findName != null && UserModel.IsPasswordMatched(model.Password, findName)) return new KeyValuePair<bool, User?>(true, findName);
            if (findEmail != null && UserModel.IsPasswordMatched(model.Password, findEmail)) return new KeyValuePair<bool, User?>(true, findEmail);
            return response;
        }
    }
}
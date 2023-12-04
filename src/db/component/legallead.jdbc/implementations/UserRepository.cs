using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly DataContext _context;
        private readonly IDapperCommand _command;

        public UserRepository(DataContext context) : base(context)
        {
            _context = context;
            _command = context.GetCommand;
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
    }
}
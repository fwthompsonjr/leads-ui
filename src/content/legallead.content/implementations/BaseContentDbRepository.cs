using legallead.content.entities;
using legallead.content.extensions;
using legallead.content.helpers;
using legallead.content.interfaces;

namespace legallead.content.implementations
{
    public abstract class BaseContentDbRepository<T> where T : CommonBaseDto, new()
    {
        protected readonly ContentDbContext _context;
        protected readonly IContentDbCommand _command;
        protected readonly T _sut = new();

        protected BaseContentDbRepository(ContentDbContext context)
        {
            _context = context;
            _command = context.GetCommand;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var sql = _sut.SelectSQL();
            return await _command.QueryAsync<T>(connection, sql);
        }

        public async Task<T?> GetById(string id)
        {
            using var connection = _context.CreateConnection();
            var parm = new T { Id = id };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await _command.QuerySingleOrDefaultAsync<T>(connection, sql, parms);
        }

        public async Task Create(T source)
        {
            using var connection = _context.CreateConnection();
            var sql = source.InsertSQL();
            var parms = source.InsertParameters();
            await _command.ExecuteAsync(connection, sql, parms);
        }

        public async Task Update(T source)
        {
            using var connection = _context.CreateConnection();
            var sql = source.UpdateByIdSQL();
            var parms = source.UpdateParameters();
            await _command.ExecuteAsync(connection, sql, parms);
        }

        public async Task Delete(string id)
        {
            using var connection = _context.CreateConnection();
            var parm = new T { Id = id };
            var sql = _sut.DeleteSQL();
            var parms = parm.DeleteParameters();
            await _command.ExecuteAsync(connection, sql, parms);
        }
    }
}
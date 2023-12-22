using legallead.logging.entities;
using legallead.logging.extensions;
using legallead.logging.interfaces;

namespace legallead.logging.implementations
{
    public abstract class BaseLoggingDbRepository<T> where T : CommonBaseDto, new()
    {
        private readonly ILoggingDbContext _context;
        private readonly ILoggingDbCommand _command;
        protected readonly T _sut = new();

        protected BaseLoggingDbRepository(ILoggingDbContext context)
        {
            _context = context;
            _command = context.GetCommand;
        }

        public virtual ILoggingDbContext GetContext => _context;
        public virtual ILoggingDbCommand GetCommand => _command;

        public async Task<IEnumerable<T>> GetAll()
        {
            using var connection = GetContext.CreateConnection();
            var sql = _sut.SelectSQL();
            return await GetCommand.QueryAsync<T>(connection, sql);
        }

        public async Task<T?> GetById(long id)
        {
            using var connection = GetContext.CreateConnection();
            var parm = new T { Id = id };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await GetCommand.QuerySingleOrDefaultAsync<T>(connection, sql, parms);
        }

        public virtual async Task Create(T source)
        {
            using var connection = GetContext.CreateConnection();
            var sql = source.InsertSQL();
            var parms = source.InsertParameters();
            await GetCommand.ExecuteAsync(connection, sql, parms);
        }

        public async Task Update(T source)
        {
            using var connection = GetContext.CreateConnection();
            var sql = source.UpdateByIdSQL();
            var parms = source.UpdateParameters();
            await GetCommand.ExecuteAsync(connection, sql, parms);
        }

        public async Task Delete(long id)
        {
            using var connection = GetContext.CreateConnection();
            var parm = new T { Id = id };
            var sql = _sut.DeleteSQL();
            var parms = parm.DeleteParameters();
            await GetCommand.ExecuteAsync(connection, sql, parms);
        }
    }
}
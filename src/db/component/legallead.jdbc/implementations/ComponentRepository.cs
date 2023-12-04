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
        private readonly Component component = new();

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
            var sql = component.SelectSQL();
            return await _command.QueryAsync<Component>(connection, sql);
        }

        public async Task<Component?> GetById(string id)
        {
            using var connection = _context.CreateConnection();
            var parm = new Component { Id = id };
            var sql = component.SelectSQL(parm);
            var parms = component.SelectParameters(parm);
            return await _command.QuerySingleOrDefaultAsync<Component>(connection, sql, parms);
        }

        public async Task<Component?> GetByName(string name)
        {
            using var connection = _context.CreateConnection();
            var parm = new Component { Name = name };
            var sql = component.SelectSQL(parm);
            var parms = component.SelectParameters(parm);
            return await _command.QuerySingleOrDefaultAsync<Component>(connection, sql, parms);
        }

        public async Task Create(Component application)
        {
            using var connection = _context.CreateConnection();
            var sql = application.InsertSQL();
            var parms = application.InsertParameters();
            await _command.ExecuteAsync(connection, sql, parms);
        }

        public async Task Update(Component application)
        {
            using var connection = _context.CreateConnection();
            var sql = application.UpdateByIdSQL(application);
            var parms = application.UpdateParameters();
            await _command.ExecuteAsync(connection, sql, parms);
        }

        public async Task Delete(string id)
        {
            using var connection = _context.CreateConnection();
            var parm = new Component { Id = id };
            var sql = component.DeleteSQL();
            var parms = parm.DeleteParameters();
            await _command.ExecuteAsync(connection, sql, parms);
        }
    }
}
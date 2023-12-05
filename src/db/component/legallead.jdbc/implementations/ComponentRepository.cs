using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class ComponentRepository : BaseRepository<Component>, IComponentRepository
    {
        public ComponentRepository(DataContext context) : base(context)
        {
            Task.Run(() =>
            {
                context.Init().ConfigureAwait(false);
            });
        }

        public async Task<Component?> GetByName(string name)
        {
            using var connection = _context.CreateConnection();
            var parm = new Component { Name = name };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await _command.QuerySingleOrDefaultAsync<Component>(connection, sql, parms);
        }
    }
}
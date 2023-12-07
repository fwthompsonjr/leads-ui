using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class PermissionMapRepository : BaseRepository<PermissionMap>, IPermissionMapRepository
    {
        public PermissionMapRepository(DataContext context) : base(context)
        {
        }
    }
}
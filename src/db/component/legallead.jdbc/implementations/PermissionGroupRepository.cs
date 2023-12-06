using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class PermissionGroupRepository : BaseRepository<PermissionGroup>, IPermissionGroupRepository
    {
        public PermissionGroupRepository(DataContext context) : base(context) { }
    }
}
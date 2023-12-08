using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class ProfileMapRepository : BaseRepository<ProfileMap>, IProfileMapRepository
    {
        public ProfileMapRepository(DataContext context) : base(context)
        {
        }
    }
}
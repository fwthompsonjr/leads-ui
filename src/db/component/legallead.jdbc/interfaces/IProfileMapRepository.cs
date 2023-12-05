using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IProfileMapRepository
    {
        Task<IEnumerable<ProfileMap>> GetAll();

        Task<ProfileMap?> GetById(string id);

        Task Create(ProfileMap user);

        Task Update(ProfileMap user);

        Task Delete(string id);
    }
}
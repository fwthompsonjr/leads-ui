using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IComponentRepository
    {
        Task<IEnumerable<Component>> GetAll();

        Task<Component?> GetById(string id);

        Task<Component?> GetByName(string name);

        Task Create(Component application);

        Task Update(Component application);

        Task Delete(string id);
    }
}
using legallead.content.entities;

namespace legallead.content.interfaces
{
    public interface IWebContentRepository
    {
        Task Insert(CreateContentRequest dto);

        Task CreateRevision(WebContentDto dto);

        Task Delete(string id);

        Task<IEnumerable<WebContentDto>> GetAll();

        Task<IEnumerable<WebContentDto>> GetAllActive();

        Task<IEnumerable<WebContentDto>> GetByInternalId(int internalId);

        Task<IEnumerable<WebContentDto>> GetByName(string name);

        Task SetActiveRevision(WebContentDto dto);

        Task Update(WebContentDto dto);
    }
}
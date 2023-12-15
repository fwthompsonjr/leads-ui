using legallead.content.entities;

namespace legallead.content.interfaces
{
    public interface IWebContentLineRepository
    {
        Task Create(WebContentLineDto dto);

        Task Delete(string id);

        Task<IEnumerable<WebContentLineDto>> GetAll();

        Task<IEnumerable<WebContentLineDto>> GetAll(WebContentDto dto);

        Task<IEnumerable<WebContentLineDto>> GetByInternalId(int internalId);

        Task Update(WebContentLineDto dto);
    }
}
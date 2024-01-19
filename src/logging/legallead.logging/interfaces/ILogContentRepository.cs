using legallead.logging.entities;

namespace legallead.logging.interfaces
{
    public interface ILogContentRepository
    {
        Task Insert(LogInsertModel dto);

        Task<IEnumerable<VwLogDto>> Query(LogQueryModel query);

        Task InsertChild(LogContentDetailDto dto);
    }
}
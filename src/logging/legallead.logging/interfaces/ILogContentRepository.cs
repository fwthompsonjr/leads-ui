using legallead.logging.entities;

namespace legallead.logging.interfaces
{
    internal interface ILogContentRepository
    {
        Task Insert(LogInsertModel dto);

        Task<IEnumerable<VwLogDto>> Query(LogQueryModel query);

        Task InsertChild(LogContentDetailDto dto);
    }
}
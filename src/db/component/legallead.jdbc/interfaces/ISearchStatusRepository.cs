using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface ISearchStatusRepository
    {
        bool Begin(WorkBeginningBo bo);
        bool Update(WorkStatusBo bo);
        IEnumerable<WorkingSearchBo>? List();
    }
}

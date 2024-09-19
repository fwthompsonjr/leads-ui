using legallead.jdbc.entities;
using legallead.jdbc.enumerations;

namespace legallead.jdbc.interfaces
{
    public interface IQueueWorkRepository
    {
        List<QueueWorkingBo> InsertRange(string json);
        QueueWorkingBo? UpdateStatus(QueueWorkingBo updateBo);
        List<QueueWorkingBo> Fetch();
        QueueWorkingUserBo? GetUserBySearchId(string? searchId);
        Task<List<StatusSummaryByCountyBo>> GetSummary(QueueStatusTypes statusType);
        Task<List<StatusSummaryBo>> GetStatus();
        Task<List<QueueNonPersonBo>> GetNonPersonData();
        QueuePersonDataBo? UpdatePersonData(QueuePersonDataBo bo);
    }
}
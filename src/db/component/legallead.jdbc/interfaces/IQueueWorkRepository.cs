using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IQueueWorkRepository
    {
        List<QueueWorkingBo> InsertRange(string json);
        QueueWorkingBo? UpdateStatus(QueueWorkingBo updateBo);
        List<QueueWorkingBo> Fetch();
    }
}
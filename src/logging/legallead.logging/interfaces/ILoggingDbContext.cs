using System.Data;

namespace legallead.logging.interfaces
{
    public interface ILoggingDbContext
    {
        ILoggingDbCommand GetCommand { get; }

        IDbConnection CreateConnection();
    }
}
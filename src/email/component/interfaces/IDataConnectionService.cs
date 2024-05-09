using System.Data;

namespace legallead.email.services
{
    public interface IDataConnectionService
    {
        IDbConnection CreateConnection();
    }
}
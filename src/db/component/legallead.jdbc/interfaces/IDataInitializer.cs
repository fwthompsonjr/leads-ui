using System.Data;

namespace legallead.jdbc.interfaces
{
    public interface IDataInitializer
    {
        IDbConnection CreateConnection();

        Task Init();
    }
}
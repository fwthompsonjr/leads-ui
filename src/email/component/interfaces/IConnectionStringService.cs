namespace legallead.email.services
{
    public interface IConnectionStringService
    {
        string ConnectionString();
        string[] GetCredential();
    }
}
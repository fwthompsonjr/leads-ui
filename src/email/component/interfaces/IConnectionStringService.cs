namespace legallead.email.services
{
    internal interface IConnectionStringService
    {
        string ConnectionString();
        string[] GetCredential();
    }
}
namespace legallead.jdbc.interfaces
{
    public interface IBgComponentRepository
    {
        Task<bool> GetStatus(string? componentName, string? serviceName);
        Task<bool> ReportHealth(string? componentName, string? serviceName, string health);
        Task<bool> SetStatus(string? componentName, string? serviceName, string status);
    }
}

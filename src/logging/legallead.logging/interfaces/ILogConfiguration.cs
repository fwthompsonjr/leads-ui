namespace legallead.logging.interfaces
{
    public interface ILogConfiguration
    {
        LogConfigurationLevel LogLevel { get; }

        void SetLoggingLevel(LogConfigurationLevel level);
    }
}
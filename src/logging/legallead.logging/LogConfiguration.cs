using legallead.logging.interfaces;

namespace legallead.logging
{
    public class LogConfiguration : ILogConfiguration
    {
        private static readonly object logLock = new();
        private LogConfigurationLevel level = LogConfigurationLevel.Information;
        public virtual LogConfigurationLevel LogLevel => level;

        public void SetLoggingLevel(LogConfigurationLevel level)
        {
            lock (logLock)
            {
                this.level = level;
            }
        }
    }
}
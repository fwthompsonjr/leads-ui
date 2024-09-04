using legallead.logging.interfaces;
using System.Runtime.CompilerServices;

namespace legallead.permissions.api.Utility
{
    public class LoggingInfrastructure : ILoggingInfrastructure
    {
        private readonly ILoggingService? _logger;

        public LoggingInfrastructure()
        {
        }

        internal LoggingInfrastructure(ILoggingService? logger)
        {
            _logger = logger;
        }

        public async Task LogCriticalAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogCritical(message, callerLineNumber, callerMethodName);
        }

        public async Task LogDebugAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogDebug(message, callerLineNumber, callerMethodName);
        }

        public async Task LogErrorAsync(Exception exception, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogError(exception, callerLineNumber, callerMethodName);
        }

        public async Task LogInformationAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogInformation(message, callerLineNumber, callerMethodName);
        }

        public async Task LogVerboseAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogVerbose(message, callerLineNumber, callerMethodName);
        }

        public async Task LogWarningAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogWarning(message, callerLineNumber, callerMethodName);
        }
    }
}
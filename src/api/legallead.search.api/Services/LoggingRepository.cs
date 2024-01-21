using legallead.logging.interfaces;
using System.Runtime.CompilerServices;

namespace legallead.search.api.Services
{
    public class LoggingRepository : ILoggingRepository
    {
        private readonly ILoggingService? _logger;
        public LoggingRepository(ILoggingService? logger)
        {
            _logger = logger;
            ClassContext = string.Empty;
        }
        public string ClassContext
        {
            get; set;
        }

        public async Task LogCritical(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogCritical(message, callerLineNumber, callerMethodName);
        }

        public async Task LogDebug(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogDebug(message, callerLineNumber, callerMethodName);
        }

        public async Task LogError(Exception exception, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogError(exception, callerLineNumber, callerMethodName);
        }

        public async Task LogInformation(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogInformation(message, callerLineNumber, callerMethodName);
        }

        public async Task LogVerbose(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogVerbose(message, callerLineNumber, callerMethodName);
        }

        public async Task LogWarning(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "")
        {
            if (_logger == null) return;
            await _logger.LogWarning(message, callerLineNumber, callerMethodName);
        }
    }
}
using System.Runtime.CompilerServices;

namespace legallead.logging.interfaces
{
    internal interface ILoggingService
    {
        Task<LogInsertModel> LogCritical(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task<LogInsertModel> LogDebug(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task<LogInsertModel> LogError(Exception exception, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task<LogInsertModel> LogInformation(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task<LogInsertModel> LogVerbose(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task<LogInsertModel> LogWarning(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
    }
}
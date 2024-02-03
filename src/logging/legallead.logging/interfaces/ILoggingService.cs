using System.Runtime.CompilerServices;

namespace legallead.logging.interfaces
{
    public interface ILoggingService
    {
        string ClassContext { get; set; }

        Task<LogInsertModel> LogCritical(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogCritical(string message, string namespaceName, string className, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogDebug(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogDebug(string message, string namespaceName, string className, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogError(Exception exception, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogError(Exception exception, string namespaceName, string className, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogInformation(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogInformation(string message, string namespaceName, string className, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogVerbose(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogVerbose(string message, string namespaceName, string className, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogWarning(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
        Task<LogInsertModel> LogWarning(string message, string namespaceName, string className, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
    }
}
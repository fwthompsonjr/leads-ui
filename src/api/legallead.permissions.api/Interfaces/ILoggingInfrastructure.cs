using System.Runtime.CompilerServices;

namespace legallead.permissions.api.Interfaces
{
    public interface ILoggingInfrastructure
    {
        Task LogCriticalAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task LogDebugAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task LogErrorAsync(Exception exception, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task LogInformationAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task LogVerboseAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");

        Task LogWarningAsync(string message, [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = "");
    }
}
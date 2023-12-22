using legallead.logging.extensions;
using legallead.logging.interfaces;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace legallead.logging
{
    internal class LoggingService
    {
        private readonly Guid _id;
        private readonly ILogContentRepository _contentRepository;

        public LoggingService(Guid? requestId = null, ILogContentRepository? contentRepository = null)
        {
            _id = requestId.GetValueOrDefault(Guid.NewGuid());
            contentRepository ??= InternalDbServiceProvider.GetService<ILogContentRepository>();
            _contentRepository = contentRepository;
        }

        private enum SeverityCodes
        {
            Verbose = -1,
            Debug = 0,
            Information = 10,
            Warning = 100,
            Critical = 1000,
            Error = 5000
        }

        public async Task<LogInsertModel> WriteVerbose(
            string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMethodName = "")
        {
            var model = GetInsertModel(SeverityCodes.Verbose, _id, callerLineNumber, callerMethodName, message);
            await _contentRepository.Insert(model);
            return model;
        }

        public async Task<LogInsertModel> WriteDebug(
            string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMethodName = "")
        {
            var model = GetInsertModel(SeverityCodes.Debug, _id, callerLineNumber, callerMethodName, message);
            await _contentRepository.Insert(model);
            return model;
        }

        public async Task<LogInsertModel> WriteInformation(
            string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMethodName = "")
        {
            var model = GetInsertModel(SeverityCodes.Information, _id, callerLineNumber, callerMethodName, message);
            await _contentRepository.Insert(model);
            return model;
        }

        public async Task<LogInsertModel> WriteWarning(
            string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMethodName = "")
        {
            var model = GetInsertModel(SeverityCodes.Warning, _id, callerLineNumber, callerMethodName, message);
            await _contentRepository.Insert(model);
            return model;
        }

        public async Task<LogInsertModel> WriteCritical(
            string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMethodName = "")
        {
            var model = GetInsertModel(SeverityCodes.Critical, _id, callerLineNumber, callerMethodName, message);
            await _contentRepository.Insert(model);
            return model;
        }

        public async Task<LogInsertModel> WriteError(
            Exception exception,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMethodName = "")
        {
            var model = GetInsertModel(SeverityCodes.Error, _id, callerLineNumber, callerMethodName, exception.Message);
            model.Detail = exception.ToString();
            await _contentRepository.Insert(model);
            return model;
        }

        internal static string GetInsertParameters(LogInsertModel dto)
        {
            var parms = new List<string>
            {
                $"'{dto.RequestId.SubstituteSingleQuote()}'",
                $"{dto.StatusId ?? 0}",
                $"{dto.LineNumber ?? 0}",
                $"'{dto.NameSpace.SubstituteSingleQuote()}'",
                $"'{dto.ClassName.SubstituteSingleQuote()}'",
                $"'{dto.MethodName.SubstituteSingleQuote()}'",
                $"'{dto.Message.SubstituteSingleQuote()}'"
            };
            return string.Join(", ", parms);
        }

        internal static string GetQueryParameters(LogQueryModel query)
        {
            var parms = new List<string>
            {
                $"{query.Id.ToSprocParameter()}",
                $"{query.RequestId.ToSprocParameter()}",
                $"{query.StatusId.ToSprocParameter()}",
                $"{query.NameSpace.ToSprocParameter()}",
                $"{query.ClassName.ToSprocParameter()}",
                $"{query.MethodName.ToSprocParameter()}",
            };
            return string.Join(", ", parms);
        }

        private static LogInsertModel GetInsertModel(SeverityCodes codes, Guid requestId, int lineNumber, string methodName, string message)
        {
            var callingMethod = new StackTrace().GetFrame(1)?.GetMethod();
            var details = CallerDetails.GetDetails(callingMethod);
            return new LogInsertModel
            {
                RequestId = requestId.ToString("D"),
                StatusId = (int)codes,
                LineNumber = lineNumber,
                NameSpace = details.NameSpace.Truncate(255),
                MethodName = methodName.Truncate(255),
                Message = message.Truncate(500),
                ClassName = details.ClassName.Truncate(255)
            };
        }

        private sealed class CallerDetails
        {
            private const string Missing = " - not located - ";

            public string NameSpace { get; set; } = Missing;
            public string ClassName { get; set; } = Missing;

            public static CallerDetails GetDetails(MethodBase? method)
            {
                var details = new CallerDetails();
                if (method == null) return details;
                var type = method.ReflectedType;
                if (type == null) return details;
                var ns = type.Namespace ?? string.Empty;
                var fullname = type.FullName ?? string.Empty;
                if (!string.IsNullOrEmpty(ns)) { details.NameSpace = ns; }
                if (!string.IsNullOrEmpty(fullname)) { details.ClassName = fullname.Replace(ns, string.Empty); }
                return details;
            }
        }
    }
}
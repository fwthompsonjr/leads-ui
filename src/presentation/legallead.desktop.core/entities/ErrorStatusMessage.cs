﻿using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class ErrorStatusMessage
    {
        public string Id { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string Code { get; set; } = string.Empty;
        public string[] Message { get; set; } = Array.Empty<string>();

        public string Description
        {
            get
            {
                if (Message.Length == 0) return string.Empty;
                var msg = string.Join(" ", Message);
                return msg;
            }
        }

        private static List<ErrorStatusMessage>? _messages;

        private static readonly ErrorStatusMessage defaultStatusMessage = new()
        {
            Id = "500",
            IsDefault = true,
            Code = "Unexpected Error",
            Message = new string[]
            {
                "Application encountered an unexpected error. ",
                "Please contact system administrator for additional details or information regarding this error.",
            }
        };

        public static List<ErrorStatusMessage> GetMessages()
        {
            if (_messages != null) return _messages;
            var messages = new List<ErrorStatusMessage>();
            var source = Properties.Resources.errorstatus_json;
            var content = TryGet<List<ErrorStatusMessage>>(source);
            messages.AddRange(content);
            if (!messages.Any()) { messages.Add(defaultStatusMessage); }
            _messages = messages;
            return _messages;
        }

        private static T TryGet<T>(string source) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(source) ?? new();
            }
            catch
            {
                return new T();
            }
        }
    }
}
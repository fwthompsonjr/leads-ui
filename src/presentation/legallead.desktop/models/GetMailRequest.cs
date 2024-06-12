using System;

namespace legallead.desktop.models
{
    internal class GetMailRequest
    {
        public string RequestType { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public DateTime? LastUpdate { get; set; }
    }
}

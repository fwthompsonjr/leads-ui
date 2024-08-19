using legallead.permissions.api.Entities;
using Newtonsoft.Json;

namespace legallead.permissions.api.Extensions
{
    public static class QueueRequestExtensions
    {
        public static bool IsValid(this QueueInitializeRequest request)
        {
            if (request == null) return false;
            if (string.IsNullOrEmpty(request.MachineName)) return false;
            if (string.IsNullOrEmpty(request.Message)) return false;
            if (!request.StatusId.HasValue) return false;
            if (request.MachineName.Length > 256) return false;
            if (request.Message.Length > 255) return false;
            if (!statuses.Contains(request.StatusId.Value)) return false;
            if (request.Items.Count == 0) return false;
            return true;
        }

        public static bool IsValid(this QueueUpdateRequest request)
        {
            if (request == null) return false;
            if (string.IsNullOrEmpty(request.Id)) return false;
            if (string.IsNullOrEmpty(request.SearchId)) return false;
            if (string.IsNullOrEmpty(request.Message)) return false;
            if (request.Message.Length > 255) return false;
            if (!request.StatusId.HasValue) return false;
            if (!statuses.Contains(request.StatusId.Value)) return false;
            if (!Guid.TryParse(request.Id, out var _)) return false;
            if (!Guid.TryParse(request.SearchId, out var _)) return false;
            if (!statuses.Contains(request.StatusId.Value)) return false;
            return true;
        }

        public static string Serialize(this QueueInitializeRequest request)
        {
            var fallback = new QueueInitializeRequest { StatusId = -100 };
            if (!request.IsValid())
            {
                return JsonConvert.SerializeObject(fallback);
            }
            request.Items.RemoveAll(x =>
            {
                if (string.IsNullOrEmpty(x.Id)) return false;
                return Guid.TryParse(x.Id, out var _);
            });
            return JsonConvert.SerializeObject(request);
        }

        public static QueueWorkingBo ConvertFrom(this QueueUpdateRequest request)
        {
            return new()
            {
                Id = request.Id,
                SearchId = request.SearchId,
                Message = request.Message,
                StatusId = request.StatusId
            };
        }
        private static readonly int[] statuses = [-1, 0, 1, 2];
    }
}

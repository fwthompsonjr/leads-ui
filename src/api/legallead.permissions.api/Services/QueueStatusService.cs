using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using Newtonsoft.Json;

namespace legallead.permissions.api.Services
{
    public class QueueStatusService(
        IQueueWorkRepository repo,
        ISearchQueueRepository queue) : IQueueStatusService
    {
        private readonly IQueueWorkRepository _repo = repo;
        private readonly ISearchQueueRepository _queue = queue;
        private readonly IMailMessageWrapper? _notificationSvc;
        internal QueueStatusService(
        IQueueWorkRepository repo,
        ISearchQueueRepository queue,
        IMailMessageWrapper notification) : this(repo, queue)
        {
            _notificationSvc = notification;
        }
        public List<QueueWorkingBo> Insert(QueueInitializeRequest request)
        {
            try
            {
                var json = request.Serialize();
                var response = _repo.InsertRange(json);
                return response;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<QueueWorkingBo?> Update(QueueUpdateRequest request)
        {
            try
            {
                if (!request.IsValid()) return null;
                var payload = request.ConvertFrom();
                var response = _repo.UpdateStatus(payload);
                await TrySendCompletionEmail(request, response);
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<QueuedRecord>> Fetch()
        {
            try
            {
                var data = await _queue.GetQueue();
                if (data.Count == 0) return [];
                var working = _repo.Fetch().Select(x => x.SearchId).Distinct().ToList();
                data = data.FindAll(d =>
                {
                    return !working.Contains(d.Id, StringComparer.OrdinalIgnoreCase);
                });
                var json = JsonConvert.SerializeObject(data);
                return JsonConvert.DeserializeObject<List<QueuedRecord>>(json) ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public accessor")]
        private async Task TrySendCompletionEmail(QueueUpdateRequest request, QueueWorkingBo? response)
        {
            if (request.StatusId.GetValueOrDefault() != 1 ||
                response == null ||
                response.CompletionDate == null ||
                _notificationSvc == null)
            {
                return;
            }
            await SendCompletionEmail(response);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public accessor")]
        private async Task SendCompletionEmail(QueueWorkingBo response)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response.SearchId)) return;
                var search = GetModel(await _queue.GetQueueItem(response.SearchId));
                var user = _repo.GetUserBySearchId(response.SearchId);
                if (search == null || user == null) return;
                _notificationSvc?.Send(search, user);
            }
            catch
            {
                // action on email send failure
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public accessor")]
        private static QueuedRecord? GetModel(SearchQueueDto? query)
        {
            try
            {
                if (query == null) return null;
                var json = JsonConvert.SerializeObject(query);
                var model = JsonConvert.DeserializeObject<QueuedRecord>(json);
                return model;
            }
            catch
            {
                return null;
            }
        }
    }
}

using legallead.jdbc;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using Newtonsoft.Json;

namespace legallead.permissions.api.Services
{
    public class QueueStatusService(
        IQueueWorkRepository repo,
        ISearchQueueRepository queue,
        ISearchStatusRepository statusDb,
        IUserSearchRepository userDb) : IQueueStatusService
    {
        private readonly IQueueWorkRepository _repo = repo;
        private readonly ISearchQueueRepository _queue = queue;
        private readonly ISearchStatusRepository _statusDb = statusDb;
        private readonly IUserSearchRepository _userDb = userDb;
        private readonly IMailMessageWrapper? _notificationSvc;
        internal QueueStatusService(
        IQueueWorkRepository repo,
        ISearchQueueRepository queue,
        ISearchStatusRepository sts,
        IUserSearchRepository userSvc,
        IMailMessageWrapper notification) : this(repo, queue, sts, userSvc)
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

        public async Task<KeyValuePair<bool, string>> Start(QueuedRecord search)
        {
            var dto = GetDto(search);
            if (dto == null) return new(false, "unmappable entity");
            var dbresponse = await _queue.Start(dto);
            return dbresponse;
        }

        public async Task Complete(QueueRecordStatusRequest request)
        {
            var uniqueId = request.UniqueId ?? string.Empty;
            await _queue.Complete(uniqueId);
        }
        public async Task GenerationComplete(QueueCompletionRequest request)
        {
            var uniqueId = request.UniqueId ?? string.Empty;
            var parameter = request.QueryParameter.ToInstance<QueueSearchItem>();
            var list = request.Data.ToInstance<List<QueuePersonItem>>();
            if (string.IsNullOrWhiteSpace(uniqueId) ||
                parameter == null ||
                list == null ||
                list.Count == 0)
            {
                await _queue.Complete(uniqueId);
                return;
            }
            var rcount = list.Count;
            var js = request.Data ?? JsonConvert.SerializeObject(list);
            _ = _userDb.Append(SearchTargetTypes.Staging, uniqueId, js, "data-output-person-addres");
            _ = _userDb.Append(SearchTargetTypes.Staging, uniqueId, rcount, "data-output-row-count");
            _ = _queue.Complete(uniqueId);
            _ = _userDb.UpdateRowCount(uniqueId, rcount);
        }

        public async Task PostStatus(QueueRecordStatusRequest request)
        {
            if (!request.IsValid()) return;
            var uniqueId = request.UniqueId ?? string.Empty;
            var messageId = request.MessageId.GetValueOrDefault(-1);
            var statusId = request.StatusId.GetValueOrDefault(-1);
            if (messageId > messages.Count - 1 || messageId < 0) { return; }
            if (statusId > statuses.Length - 1 || statusId < 0) { return; }
            var statusCode = statuses[statusId];
            var message = string.Format(messages[messageId], "queue record api service", statusCode);
            var dbresponse = await _queue.Status(uniqueId, message);
            if (!dbresponse.Key) return;

            var bo = new WorkStatusBo { Id = uniqueId, MessageId = messageId, StatusId = statusId };
            _statusDb.Update(bo);
        }

        public async Task Content(string id, byte[] bytes)
        {
            await _queue.Content(id, bytes);
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

        private static SearchQueueDto? GetDto(QueuedRecord record)
        {
            try
            {
                if (record == null) return null;
                var json = JsonConvert.SerializeObject(record);
                var model = JsonConvert.DeserializeObject<SearchQueueDto>(json);
                return model;
            }
            catch
            {
                return null;
            }

        }


        private static readonly List<string> messages =
        [
            $"{0}: process beginning: {1}", // 0
            $"{0}: parameter evaluation: {1}", // 1
            $"{0}: parameter conversion to search request: {1}", // 2
            $"{0}: search request processing: {1}", // 3
            $"{0}: excel content conversion: {1}", // 4
            $"{0}: excel content serialization: {1}", // 5
            $"{0}: process complete: {1}", // 6
        ];

        internal static readonly string[] statuses = ["begin", "complete", "failed"];



    }
}

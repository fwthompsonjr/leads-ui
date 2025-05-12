using AutoMapper;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using StructureMap.Query;

namespace legallead.permissions.api.Services
{
    public class UserUsageService(IUserUsageRepository repository) : IUserUsageService
    {
        private readonly IUserUsageRepository db = repository;
        private static readonly IMapper mapper = UserUsageMapper.Mapper;
        public async Task<AppendUsageRecordResponse?> AppendUsageRecordAsync(AppendUsageRecordRequest model)
        {
            var request = mapper.Map<UserUsageAppendRecordModel>(model);
            var response = await db.AppendUsageRecord(request);
            if (response == null) return new();
            var dest = mapper.Map<AppendUsageRecordResponse>(response);
            return dest;
        }

        public async Task<CompleteUsageRecordResponse> CompleteUsageRecordAsync(CompleteUsageRecordRequest model)
        {
            var request = mapper.Map<UserUsageCompleteRecordModel>(model);
            var response = await db.CompleteUsageRecord(request);
            var dest = mapper.Map<CompleteUsageRecordResponse>(model);
            dest.IsCompleted = response.Key;
            dest.Message = response.Value;
            return dest;
        }

        public async Task<GetMonthlyLimitResponse> GetMonthlyLimitAsync(GetMonthlyLimitRequest model)
        {
            var response = mapper.Map<GetMonthlyLimitResponse>(model);
            response.Content =
                model.GetAllCounties ?
                Serialize(await db.GetMonthlyLimit(model.LeadId)) :
                Serialize(await db.GetMonthlyLimit(model.LeadId, model.CountyId));
            return response;
        }

        public async Task<GetUsageResponse> GetUsageAsync(GetUsageRequest model)
        {
            var response = mapper.Map<GetUsageResponse>(model);
            response.Content = Serialize(await db.GetUsage(model.LeadId, model.SearchDate, model.GetAllCounties));
            return response;
        }

        public async Task<GetUsageResponse> GetUsageSummaryAsync(GetUsageRequest model)
        {
            var response = mapper.Map<GetUsageResponse>(model);
            response.Content = Serialize(await db.GetUsageSummary(model.LeadId, model.SearchDate, model.GetAllCounties));
            return response;
        }

        public async Task<GetExcelDetailByIdResponse> GetExcelDetailAsync(string id)
        {
            var dto = await db.GetUsageFileDetails(id);
            var response = new GetExcelDetailByIdResponse
            {
                Id = id,
                IsCompleted = dto.IsCompleted,
                Name = dto.Name,
                Password = dto.Password,
            };
            return response;
        }

        public async Task<SetMonthlyLimitResponse> SetMonthlyLimitAsync(SetMonthlyLimitRequest model)
        {
            var response = mapper.Map<SetMonthlyLimitResponse>(model);
            response.Content = Serialize(await db.SetMonthlyLimit(model.LeadId, model.CountyId, model.MonthLimit));
            return response;
        }

        public async Task<OfflineDataModel> AppendOfflineRecordAsync(OfflineDataModel model)
        {
            var request = mapper.Map<OfflineRequestModel>(model);
            var data = await db.OfflineRequestBeginAsync(request);
            if (data == null) return model;
            model.OfflineId = data.OfflineId;
            return model;
        }

        public async Task<OfflineDataModel> UpdateOfflineRecordAsync(OfflineDataModel model)
        {
            var request = mapper.Map<OfflineRequestModel>(model);
            await db.OfflineRequestUpdateAsync(request);
            return model;
        }

        public async Task<IEnumerable<UserOfflineStatusResponse>?> GetOfflineStatusAsync(string userId)
        {
            // update search types before fetching offline statuses
            await UpdateOfflineSearchTypesAsync();
            var response = await db.GetOfflineStatusAsync(userId);
            if (response == null) return default;
            var json = Serialize(response);
            return JsonConvert.DeserializeObject<List<UserOfflineStatusResponse>>(json);
        }

        public async Task<bool> SetOfflineCourtTypeAsync(OfflineDataModel model)
        {
            var request = mapper.Map<OfflineRequestModel>(model);
            var updated = await db.OfflineRequestSetCourtTypeAsync(request);
            return updated;
        }
        public async Task<bool> TerminateOfflineRequestAsync(OfflineDataModel model)
        {
            var request = mapper.Map<OfflineRequestModel>(model);
            var updated = await db.OfflineRequestTerminateAsync(request);
            return updated;
        }

        public async Task<string> GetDownloadStatusAsync(OfflineDataModel model)
        {
            var request = mapper.Map<OfflineRequestModel>(model);
            var response = (await db.OfflineRequestCanDownload(request)) ?? new();
            var json = Serialize(response);
            return json;
        }
        public async Task<string> SetDownloadCompletedAsync(OfflineDataModel model)
        {
            var request = new OfflineDownloadModel
            {
                Id = model.OfflineId,
                RequestId = model.RequestId,
                CanDownload = true,
                Workload = model.Message
            };
            var response = await db.OfflineRequestFlagAsDownloadedAsync(request);
            await UpdateOfflineHistoryAsync();
            return new
            {
                model.RequestId,
                IsCompleted = response
            }.ToJsonString();
        }

        public async Task UpdateOfflineHistoryAsync()
        {
            await UpdateOfflineSearchTypesAsync();
            await db.OfflineRequestSyncHistoryAsync();
        }
        public async Task UpdateOfflineSearchTypesAsync()
        {
            await db.OfflineRequestSetSearchTypeAsync();
        }

        public async Task<List<OfflineSearchTypeBo>> GetOfflineSearchTypesByIdAsync(string leadId)
        {
            var data = await db.GetOfflineGetSearchTypeAsync(leadId);
            if (data == null || data.Count == 0) return [];
            return DeSerialize<List<OfflineSearchTypeBo>>(Serialize(data)) ?? [];
        }
        public async Task<string?> FindCaseItemByCaseNumberAsync(int countyId, string caseNumber)
        {
            var model = new OfflineCaseItemModel { CountyId = countyId, CaseNumber = caseNumber };
            var data = await db.OfflineFindByCaseNumber(model);
            if (data == null) return null;
            return data.ToJsonString();
        }
        public async Task<List<OfflineDataModel>?> GetOfflineWorkQueueAsync()
        {
            var data = await db.GetOfflineWorkQueueAsync();
            if (data == null || data.Count == 0) return [];
            return DeSerialize<List<OfflineDataModel>>(Serialize(data)) ?? [];
        }
        private static string Serialize(object? value)
        {
            if (value == null) return string.Empty;
            return JsonConvert.SerializeObject(value);
        }
        private static T? DeSerialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}

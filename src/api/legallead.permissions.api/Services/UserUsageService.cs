using AutoMapper;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;
using Newtonsoft.Json;

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

        private static string Serialize(object? value)
        {
            if (value == null) return string.Empty;
            return JsonConvert.SerializeObject(value);
        }
    }
}

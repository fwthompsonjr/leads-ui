using AutoMapper;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Services
{
    public class UserUsageService(IUserUsageRepository repository) : IUserUsageService
    {
        private readonly IUserUsageRepository db = repository;
        private static IMapper mapper = UserUsageMapper.Mapper;
        public async Task<AppendUsageRecordResponse?> AppendUsageRecordAsync(AppendUsageRecordRequest model)
        {
            var request = mapper.Map<UserUsageAppendRecordModel>(model);
            var response = await db.AppendUsageRecord(request);
            if (response == null) return null;
            var dest = mapper.Map<AppendUsageRecordResponse>(response);
            return dest;
        }

        public async Task<CompleteUsageRecordResponse> CompleteUsageRecordAsync(CompleteUsageRecordRequest model)
        {
            var request = mapper.Map<UserUsageAppendRecordModel>(model);
            var response = await db.CompleteUsageRecord(request);
            if (response == null) return new();
            var dest = mapper.Map<AppendUsageRecordResponse>(response);
            return dest;
        }

        public Task<GetMonthlyLimitResponse> GetMonthlyLimitAsync(GetMonthlyLimitRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<GetUsageResponse> GetUsageAsync(GetUsageRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<GetUsageResponse> GetUsageSummaryAsync(GetUsageRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<SetMonthlyLimitResponse> SetMonthlyLimitAsync(SetMonthlyLimitRequest model)
        {
            throw new NotImplementedException();
        }
    }
}

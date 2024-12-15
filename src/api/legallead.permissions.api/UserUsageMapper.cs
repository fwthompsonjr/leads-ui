using AutoMapper;
using legallead.permissions.api.Models;

namespace legallead.permissions.api
{
    public static class UserUsageMapper
    {

        private static MapperConfiguration? _configuration;
        private static IMapper? _mapper;
        private static MapperConfiguration Configuration => _configuration ??= GetConfiguration();
        public static IMapper Mapper => _mapper ??= Configuration.CreateMapper();

        private static MapperConfiguration GetConfiguration()
        {
            return new MapperConfiguration(c =>
            {
                c.CreateMap<AppendUsageRecordRequest, UserUsageAppendRecordModel>();
                c.CreateMap<CompleteUsageRecordRequest, UserUsageCompleteRecordModel>();
                c.CreateMap<GetMonthlyLimitRequest, GetMonthlyLimitResponse>();
                c.CreateMap<GetUsageRequest, GetUsageResponse>();
                c.CreateMap<SetMonthlyLimitRequest, SetMonthlyLimitResponse>();

                c.CreateMap<DbCountyAppendLimitBo, AppendUsageRecordResponse>();
                c.CreateMap<CompleteUsageRecordRequest, CompleteUsageRecordResponse>()
                    .ForMember(x => x.Id, y => y.MapFrom(m => m.UsageRecordId))
                    .ForMember(x => x.IsCompleted, y => y.MapFrom(m => false))
                    .ForMember(x => x.Message, y => y.MapFrom(m => string.Empty));
            });
        }
    }
}
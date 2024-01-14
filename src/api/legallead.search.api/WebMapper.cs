using AutoMapper;
using legallead.models.Search;
using legallead.permissions.api.Model;

namespace legallead.search.api
{
    public static class WebMapper
    {

        private static MapperConfiguration? _configuration;
        private static IMapper? _mapper;

        private static MapperConfiguration Configuration => _configuration ??= GetConfiguration();
        private static IMapper Mapper => _mapper ??= Configuration.CreateMapper();

        private static MapperConfiguration GetConfiguration()
        {
            return new MapperConfiguration(c =>
            {
                c.CreateMap<UserSearchRequest, SearchRequest>()
                    .ForMember(x => x.WebId, opt => opt.MapFrom(y => GetWebIndex(y.County.Name, y.State)))
                    .ForMember(x => x.County, opt => opt.MapFrom(y => y.County.Name))
                    .ForMember(x => x.State, opt => opt.MapFrom(y => y.State))
                    .ForMember(x => x.StartDate, opt => opt.MapFrom(y => UnixDateToString(y.StartDate)))
                    .ForMember(x => x.EndDate, opt => opt.MapFrom(y => UnixDateToString(y.EndDate)));
            });
        }
        private static int GetWebIndex(string? county, string? st)
        {
            if(string.IsNullOrWhiteSpace(county) || 
                string.IsNullOrWhiteSpace(st)) return 0;
            var lookup = $"{st.ToLower()}-{county.Replace(' ', '-').ToLower()}";
            if (lookup.Equals("tx-collin")) return 20;
            if (lookup.Equals("tx-tarrant")) return 10;
            return 0;
        }
        private static string UnixDateToString(long unixTime)
        {
            var dte = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).Date;
            return dte.ToString("yyyy-MM-dd");
        }

        public static T? MapFrom<TK, T>(TK source) where T : class, new()
        {
            if (source == null) return default;
            return Mapper.Map<T>(source);
        }

    }
}

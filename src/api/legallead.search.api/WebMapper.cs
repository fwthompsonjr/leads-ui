using AutoMapper;
using legallead.models.Search;
using legallead.permissions.api.Model;

namespace legallead.search.api
{
    public static class WebMapper
    {

        public static T? MapFrom<TK, T>(TK source) where T : class, new()
        {
            if (source == null) return default;
            return Mapper.Map<T>(source);
        }

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

                c.CreateMap<UserSearchRequest, SearchNavigationParameter>()
                    .ConvertUsing(ConvertTo);
            });
        }
        private static int GetWebIndex(string? county, string? st)
        {
            if (string.IsNullOrWhiteSpace(county) ||
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
        private static SearchNavigationParameter ConvertTo(UserSearchRequest source, SearchNavigationParameter dest)
        {
            dest ??= new();
            dest.Id = GetWebIndex(source.County.Name, source.State);
            dest.StartDate = DateTimeOffset.FromUnixTimeMilliseconds(source.StartDate).Date;
            dest.EndDate = DateTimeOffset.FromUnixTimeMilliseconds(source.EndDate).Date;
            if (dest.Id == 0)
            {
                DentonCountyNavigationMap(source, dest);
            }
            return dest;
        }

        private static void DentonCountyNavigationMap(UserSearchRequest source, SearchNavigationParameter dest)
        {
            var accepted = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15".Split(',');
            var cbxIndex = source.Details.Find(x => x.Name == "Court Type")?.Value ?? accepted[0];
            if (!accepted.Contains(cbxIndex)) cbxIndex = accepted[0];
            var keyZero = new SearchNavigationKey { Name = "SearchComboIndex", Value = cbxIndex };
            var caseSearch = new SearchNavigationKey
            {
                Name = "CaseSearchType",
                Value = dentonLinkMap["0"]
            };
            var districtSearch = new SearchNavigationKey
            {
                Name = "DistrictSearchType",
                Value = "/html/body/table/tbody/tr[2]/td/table/tbody/tr[1]/td[2]/a[2]"
            };
            // add key for combo-index
            dest.Keys.Add(keyZero);
            // add key for case-search
            var caseSearchType = source.Details.Find(x => x.Name == "Case Type")?.Value ?? "0";
            if (caseSearchType != "0" && dentonLinkMap.ContainsKey(caseSearchType))
            {
                caseSearch.Value = dentonLinkMap[caseSearchType];
            }
            dest.Keys.Add(caseSearch);
            if (caseSearchType == "2")
            {
                dest.Keys.Add(districtSearch);
            }
        }

        private static readonly Dictionary<string, string> dentonLinkMap = new() {
                { "0", "//a[@class='ssSearchHyperlink'][contains(text(),'County Court: Civil, Family')]" },
                { "1", "//a[@class='ssSearchHyperlink'][contains(text(),'Criminal Case Records')]" },
                { "2", "//a[@class='ssSearchHyperlink'][contains(text(),'District Court Case')]" }
            };
    }
}

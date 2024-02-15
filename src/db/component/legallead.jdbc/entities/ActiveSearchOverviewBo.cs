using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class ActiveSearchOverviewBo
    {
        public IEnumerable<SearchSummaryBo>? Searches { get; set; }
        public IEnumerable<SearchStatusSummaryBo>? Statuses { get; set; }
        public IEnumerable<SearchStagingSummaryBo>? Staged { get; set; }

        internal static ActiveSearchOverviewBo FromDto(ActiveSearchOverviewDto dto)
        {
            var searches =
                string.IsNullOrEmpty(dto.Searches) ?
                Array.Empty<SearchSummaryBo>() :
                JsonConvert.DeserializeObject<SearchSummaryBo[]>(dto.Searches);
            var statuses =
                string.IsNullOrEmpty(dto.Statuses) ?
                Array.Empty<SearchStatusSummaryBo>() :
                JsonConvert.DeserializeObject<SearchStatusSummaryBo[]>(dto.Statuses);
            var staged =
                string.IsNullOrEmpty(dto.Staged) ?
                Array.Empty<SearchStagingSummaryBo>() :
                JsonConvert.DeserializeObject<SearchStagingSummaryBo[]>(dto.Staged);
            return new()
            {
                Searches = searches,
                Statuses = statuses,
                Staged = staged
            };
        }

    }
}

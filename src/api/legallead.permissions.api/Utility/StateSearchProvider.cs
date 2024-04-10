using legallead.json.db;
using legallead.models;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using UsState = legallead.json.db.entity.UsState;
using UsStateCounty = legallead.json.db.entity.UsStateCounty;

namespace legallead.permissions.api.Utility
{
    public class StateSearchProvider : IStateSearchProvider
    {
        public StateSearchProvider()
        {
            UsState.Initialize();
            UsStateCounty.Initialize();
        }

        public List<StateSearchData> GetStates()
        {
            var states = UsStatesList.All.FindAll(x => x.IsActive)
                .Select(s => new StateSearchData
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    ShortName = s.ShortName
                }).ToList();
            var counties = UsStateCountyList.All
                .FindAll(x => x.IsActive)
                .Select(s => new CountySearchData
                {
                    Index = s.Index,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    ShortName = s.ShortName,
                    StateCode = s.StateCode,
                }).ToList();
            counties.ForEach(c => c.Data = LookupSetting(c.StateCode, c.Name));
            states.ForEach(s => s.Counties = LookupCounty(s, counties));
            return states;
        }

        private static List<CountySearchData>? LookupCounty(StateSearchData s, List<CountySearchData> source)
        {
            return source.FindAll(x => x.StateCode == s.ShortName && x.IsActive);
        }

        private static CountySearchDetail? LookupSetting(string? stateCode, string? countyName)
        {
            if (string.IsNullOrWhiteSpace(stateCode)) { return null; }
            if (string.IsNullOrWhiteSpace(countyName)) { return null; }
            var county = countyName.ToLower().Replace('-', ' ');
            var itemCode = $"{stateCode.ToLower()}-{county}";
            if (!DetailList.ContainsKey(itemCode)) { return null; }
            return DetailList[itemCode];
        }

        private static Dictionary<string, CountySearchDetail> DetailList =>
            _detailList ??= GetDetails();

        private static readonly Dictionary<string, string> CountyList = new()
        {
            { "tx-collin", Properties.Resources.tx_collin_drop_down },
            { "tx-denton", Properties.Resources.tx_denton_drop_down },
            { "tx-tarrant", Properties.Resources.tx_tarrant_drop_down },
        };

        private static Dictionary<string, CountySearchDetail>? _detailList;

        private static Dictionary<string, CountySearchDetail> GetDetails()
        {
            var data = new Dictionary<string, CountySearchDetail>();
            var list = CountyList.Keys.ToList();
            list.ForEach(c =>
            {
                var serialized = CountyList[c];
                var detail = TryMap<CountySearchDetail>(serialized);
                if (detail != null) data[c] = detail;
            });
            return data;
        }

        [ExcludeFromCodeCoverage]
        private static T? TryMap<T>(string serialized)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(serialized);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
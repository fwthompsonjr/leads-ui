using legallead.json.db.entity;

namespace legallead.permissions.api.Model
{
    public class CountySearchData : UsStateCounty
    {
        public CountySearchDetail? Data { get; set; }
    }
}
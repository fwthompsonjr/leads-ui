using legallead.models;

namespace legallead.permissions.api.Model
{
    public class CountySearchData : UsStateCounty
    {
        public CountySearchDetail? Data { get; set; }
    }
}
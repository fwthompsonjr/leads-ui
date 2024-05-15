using legallead.installer.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace legallead.installer.Bo
{
    [ExcludeFromCodeCoverage(Justification = "Item scheduled for deletion")]
    internal class ReleaseModelStorageBo : ModelStorageBo<ReleaseModelBo>
    {

        public List<ReleaseModel> Models()
        {
            var list = new List<ReleaseModel>();
            if (Detail.Count == 0) return list;
            Detail.ForEach(d =>
            {
                var serialized = JsonConvert.SerializeObject(d);
                var tmp = JsonConvert.DeserializeObject<ReleaseModel>(serialized);
                if (tmp != null)
                {
                    var dte = d.PublishDate.Replace("T", " ");
                    tmp.PublishDate = DateTime.Parse(dte, CultureInfo.InvariantCulture);
                    list.Add(tmp);
                }
            });
            return list;
        }
    }
}

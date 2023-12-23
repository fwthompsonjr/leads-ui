using Newtonsoft.Json;
using System.Text;

namespace legallead.json.db.entity
{
    public class UsState
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public bool IsActive { get; set; }

        public static void Initialize()
        {
            var list = JsonConvert.DeserializeObject<List<UsState>>(GetList) ?? new();
            UsStatesList.Populate(list);
        }

        private static string? _list;
        private static string GetList => _list ??= GetJsonList();

        private static string GetJsonList()
        {
            var sb = new StringBuilder(); sb.AppendLine("[");
            sb.AppendLine("\t{ `ShortName`: `AK`, `Name`: `Alaska`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `AL`, `Name`: `Alabama`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `AR`, `Name`: `Arkansas`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `AS`, `Name`: `American Samoa`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `AZ`, `Name`: `Arizona`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `CA`, `Name`: `California`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `CO`, `Name`: `Colorado`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `CT`, `Name`: `Connecticut`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `DC`, `Name`: `District of Columbia`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `DE`, `Name`: `Delaware`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `FL`, `Name`: `Florida`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `GA`, `Name`: `Georgia`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `GU`, `Name`: `Guam`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `HI`, `Name`: `Hawaii`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `IA`, `Name`: `Iowa`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `ID`, `Name`: `Idaho`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `IL`, `Name`: `Illinois`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `IN`, `Name`: `Indiana`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `KS`, `Name`: `Kansas`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `KY`, `Name`: `Kentucky`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `LA`, `Name`: `Louisiana`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `MA`, `Name`: `Massachusetts`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `MD`, `Name`: `Maryland`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `ME`, `Name`: `Maine`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `MI`, `Name`: `Michigan`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `MN`, `Name`: `Minnesota`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `MO`, `Name`: `Missouri`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `MP`, `Name`: `Northern Mariana Islands`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `MS`, `Name`: `Mississippi`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `MT`, `Name`: `Montana`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `NC`, `Name`: `North Carolina`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `ND`, `Name`: `North Dakota`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `NE`, `Name`: `Nebraska`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `NH`, `Name`: `New Hampshire`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `NJ`, `Name`: `New Jersey`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `NM`, `Name`: `New Mexico`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `NV`, `Name`: `Nevada`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `NY`, `Name`: `New York`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `OH`, `Name`: `Ohio`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `OK`, `Name`: `Oklahoma`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `OR`, `Name`: `Oregon`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `PA`, `Name`: `Pennsylvania`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `PR`, `Name`: `Puerto Rico`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `RI`, `Name`: `Rhode Island`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `SC`, `Name`: `South Carolina`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `SD`, `Name`: `South Dakota`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `TN`, `Name`: `Tennessee`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `TX`, `Name`: `Texas`, `IsActive`: true },");
            sb.AppendLine("\t{ `ShortName`: `UT`, `Name`: `Utah`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `VA`, `Name`: `Virginia`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `VI`, `Name`: `Virgin Islands`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `VT`, `Name`: `Vermont`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `WA`, `Name`: `Washington`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `WI`, `Name`: `Wisconsin`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `WV`, `Name`: `West Virginia`, `IsActive`: false },");
            sb.AppendLine("\t{ `ShortName`: `WY`, `Name`: `Wyoming`, `IsActive`: false }");
            sb.AppendLine("]");
            sb.Replace('`', '"');
            return sb.ToString();
        }
    }
}
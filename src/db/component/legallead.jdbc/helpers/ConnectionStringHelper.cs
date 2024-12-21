using Newtonsoft.Json;

namespace legallead.jdbc.helpers
{
    internal static class ConnectionStringHelper
    {
        internal static string GetDbName(string databaseName = "app")
        {
            if (GetCurrent.ConnectId == 1) return GetCurrent.DataSource;
            return databaseName;
        }
        internal static string GetConfiguration()
        {
            if (GetCurrent.ConnectId == 1) return GetOxConfiguration();
            if (!string.IsNullOrEmpty(jsBaseConfiguration)) return jsBaseConfiguration;
            var content = ""
                          + nwl
                          + "{"
                          + nwl
                          + "\t'target': 'local',"
                          + nwl
                          + "\t'endpoints':"
                          + nwl
                          + "\t{"
                          + nwl
                          + "\t\t'local': 'db-lead-restored.cmmu8tkizri9.us-east-2.rds.amazonaws.com',"
                          + nwl
                          + "\t\t'test': 'db-lead-restored.cmmu8tkizri9.us-east-2.rds.amazonaws.com',"
                          + nwl
                          + "\t\t'production': 'db-lead-restored.cmmu8tkizri9.us-east-2.rds.amazonaws.com'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'passcodes':"
                          + nwl
                          + "\t{"
                          + nwl
                          + "\t\t'local': 'legal.lead.test.passcode',"
                          + nwl
                          + "\t\t'test': 'legal.lead.test.passcode',"
                          + nwl
                          + "\t\t'production': 'legal.lead.test.passcode'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'secrets': {"
                          + nwl
                          + "\t\t'local': 'm/1Ata5kMzR+oEX0y8RHgOvav+k4LuEIbnSJDbhA2G5pMywLOb3eBufnQzSEN+ef',"
                          + nwl
                          + "\t\t'test': 'm/1Ata5kMzR+oEX0y8RHgOvav+k4LuEIbnSJDbhA2G5pMywLOb3eBufnQzSEN+ef',"
                          + nwl
                          + "\t\t'production': 'm/1Ata5kMzR+oEX0y8RHgOvav+k4LuEIbnSJDbhA2G5pMywLOb3eBufnQzSEN+ef'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'keys': {"
                          + nwl
                          + "\t\t'local': 'CDODj4oxaiJKcKJEIBHnHw==',"
                          + nwl
                          + "\t\t'test': 'CDODj4oxaiJKcKJEIBHnHw==',"
                          + nwl
                          + "\t\t'production': 'CDODj4oxaiJKcKJEIBHnHw=='"
                          + nwl
                          + "\t}"
                          + nwl
                          + "}";
            jsBaseConfiguration = content.Replace("'", '"'.ToString());
            return jsBaseConfiguration;
        }

        private static string GetOxConfiguration()
        {
            if (!string.IsNullOrEmpty(jsOxfordConfiguration)) return jsOxfordConfiguration;
            var content = ""
                          + nwl
                          + "{"
                          + nwl
                          + "\t'target': 'local',"
                          + nwl
                          + "\t'endpoints':"
                          + nwl
                          + "\t{"
                          + nwl
                          + "\t\t'local': 'oxford-leads-db.cmmu8tkizri9.us-east-2.rds.amazonaws.com',"
                          + nwl
                          + "\t\t'test': 'oxford-leads-db.cmmu8tkizri9.us-east-2.rds.amazonaws.com',"
                          + nwl
                          + "\t\t'production': 'oxford-leads-db.cmmu8tkizri9.us-east-2.rds.amazonaws.com'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'passcodes':"
                          + nwl
                          + "\t{"
                          + nwl
                          + "\t\t'local': 'oxford.leg.test.passcode',"
                          + nwl
                          + "\t\t'test': 'oxford.leg.test.passcode',"
                          + nwl
                          + "\t\t'production': 'oxford.leg.test.passcode'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'secrets': {"
                          + nwl
                          + "\t\t'local': 'ADSzuGkWfX/qgbgcGE27xrCehFgm8/LyIVkFcLxaQZAbt0RNgXCyEJvA+e2Khnfx',"
                          + nwl
                          + "\t\t'test': 'ADSzuGkWfX/qgbgcGE27xrCehFgm8/LyIVkFcLxaQZAbt0RNgXCyEJvA+e2Khnfx',"
                          + nwl
                          + "\t\t'production': 'ADSzuGkWfX/qgbgcGE27xrCehFgm8/LyIVkFcLxaQZAbt0RNgXCyEJvA+e2Khnfx'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'keys': {"
                          + nwl
                          + "\t\t'local': 'byW4Zn05UC02YVJm42hiIQ==',"
                          + nwl
                          + "\t\t'test': 'byW4Zn05UC02YVJm42hiIQ==',"
                          + nwl
                          + "\t\t'production': 'byW4Zn05UC02YVJm42hiIQ=='"
                          + nwl
                          + "\t}"
                          + nwl
                          + "}";
            jsOxfordConfiguration = content.Replace("'", '"'.ToString());
            return jsOxfordConfiguration;

        }

        private static AppDbSetting GetCurrent
        {
            get
            {
                if (current != null) return current;
                current = JsonConvert.DeserializeObject<AppDbSetting>(connectConfig) ?? new();
                return current;
            }
        }

        private static AppDbSetting? current;
        private sealed class AppDbSetting
        {
            [JsonProperty("dbname")] public string DataSource { get; set; } = string.Empty;
            [JsonProperty("connect_index")] public int ConnectId { get; set; }
        }

        private static string jsBaseConfiguration = string.Empty;
        private static string jsOxfordConfiguration = string.Empty;
        private static readonly string connectConfig = Properties.Resources.application_db_settings;
        private static readonly string nwl = Environment.NewLine;
    }
}

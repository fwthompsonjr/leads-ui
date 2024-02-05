namespace legallead.jdbc.helpers
{
    internal static class ConnectionStringHelper
    {
        internal static string GetConfiguration()
        {
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
            return content.Replace("'", '"'.ToString());
        }
        private static readonly string nwl = Environment.NewLine;
    }
}

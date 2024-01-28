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
                          + "\t\t'local': '127.0.0.1',"
                          + nwl
                          + "\t\t'test': 'db-lead-restored.cmmu8tkizri9.us-east-2.rds.amazonaws.com',"
                          + nwl
                          + "\t\t'production': '35.224.202.163'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'passcodes':"
                          + nwl
                          + "\t{"
                          + nwl
                          + "\t\t'local': 'legal.lead.home.passcode',"
                          + nwl
                          + "\t\t'test': 'legal.lead.test.passcode',"
                          + nwl
                          + "\t\t'production': 'legal.lead.last.passcode'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'secrets': {"
                          + nwl
                          + "\t\t'local': 'jGJEauBDBO757dZo/eYL4iC1PUMsi8R5i0mBNXZh1YbsRt8aKWC3ELIheVqvHabU',"
                          + nwl
                          + "\t\t'test': 'm/1Ata5kMzR+oEX0y8RHgOvav+k4LuEIbnSJDbhA2G5pMywLOb3eBufnQzSEN+ef',"
                          + nwl
                          + "\t\t'production': 'V25lHE5JZtZf8/D95vzjzbZT5AYqQ0gskx9mg6Pi5ILx+QJnvyTTf+y7Y1uHbwz1'"
                          + nwl
                          + "\t},"
                          + nwl
                          + "\t'keys': {"
                          + nwl
                          + "\t\t'local': '0gNw7pB5/fEqq2UFihpqAw==',"
                          + nwl
                          + "\t\t'test': 'CDODj4oxaiJKcKJEIBHnHw==',"
                          + nwl
                          + "\t\t'production': 'z+0Qcqm6XJp1Uq+STRQ4Fg=='"
                          + nwl
                          + "\t}"
                          + nwl
                          + "}";
            return content.Replace("'", '"'.ToString());
        }
        private static readonly string nwl = Environment.NewLine;
    }
}

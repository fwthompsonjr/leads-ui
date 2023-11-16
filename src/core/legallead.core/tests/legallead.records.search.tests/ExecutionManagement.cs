using System.Configuration;

namespace legallead.records.search.Tests
{
    public static class ExecutionManagement
    {


        public static bool CanExecuteFetch()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                return false;
            }
            var settingCanExecute =
                ConfigurationManager.AppSettings["allow.web.integration"];
            if (settingCanExecute == null)
            {
                return true;
            }

            if (!bool.TryParse(settingCanExecute, out bool canExec))
            {
                return true;
            }

            return canExec;

        }
    }
}

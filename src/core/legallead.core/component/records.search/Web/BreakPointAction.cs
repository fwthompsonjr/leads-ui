using legallead.records.search.Dto;

namespace legallead.records.search.Web
{
    public class BreakPointAction : ElementActionBase
    {
        private const string actionName = "break-point";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            System.Diagnostics.Debugger.Break();
        }
    }
}
using legallead.records.search.Dto;

namespace legallead.records.search.Web
{
    public class DefaultAction : ElementActionBase
    {
        private const string actionName = "default";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            // do nothing
        }
    }
}
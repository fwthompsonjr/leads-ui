using legallead.records.search.DriverFactory;
using legallead.records.search.Dto;
using OpenQA.Selenium;

namespace legallead.records.search.Db
{
    [DataAction(Name = "header", ProcessId = 0, IsShared = true)]
    public class GetBrowserAction : BaseAction
    {
        public GetBrowserAction(HccProcess process) : base(process)
        {
        }

        public override TimeSpan EstimatedDuration => TimeSpan.FromSeconds(15);

        public override void Execute(IProgress<HccProcess> progress)
        {
            ReportProgress = progress;
            Start();
            WebDriver = GetDriver(false);
            End();
        }

        private IWebDriver GetDriver(bool headless)
        {
            var wdriver = (new WebDriverDto().Get()).WebDrivers;
            var driver = wdriver.Drivers.Where(d => d.Id == wdriver.SelectedIndex).FirstOrDefault();
            var container = WebDriverContainer.GetContainer;
            var provider = container.GetInstance<IWebDriverProvider>(driver.Name);
            return provider.GetWebDriver(headless);
        }
    }
}
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

        private static IWebDriver GetDriver(bool headless)
        {
            WebDrivers wdriver = (new WebDriverDto().Get()).WebDrivers;
            Driver? driver = wdriver.Drivers.FirstOrDefault(d => d.Id == wdriver.SelectedIndex);
            StructureMap.Container container = WebDriverContainer.GetContainer;
            IWebDriverProvider provider = container.GetInstance<IWebDriverProvider>(driver.Name);
            return provider.GetWebDriver(headless);
        }
    }
}
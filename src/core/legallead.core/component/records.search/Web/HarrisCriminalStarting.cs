using legallead.records.search.DriverFactory;
using legallead.records.search.Dto;
using legallead.records.search.Tools;
using OpenQA.Selenium;
using System.Diagnostics;

namespace legallead.records.search.Web
{
    public enum CriminalStartType
    {
        All,
        CaseTypes,
        Download
    }

    /// <summary>
    /// Class definition for <cref="HarrisCriminalStarting">HarrisCriminalStarting</cref> class
    /// which is used to prepare application with needed datasets and initialize data upon start
    /// </summary>
    public static class HarrisCriminalStarting
    {
        private static readonly IEnumerable<CriminalStartType> DefaultStart = GetDefaultStartType();

        private static IEnumerable<CriminalStartType> GetDefaultStartType()
        {
            return new List<CriminalStartType> { CriminalStartType.All }.AsEnumerable();
        }

        public static async Task StartAsync(IEnumerable<CriminalStartType> startTypes = null)
        {
            startTypes ??= DefaultStart;
            IWebDriver driver = GetDriver(true);
            try
            {
                // get criminal records for last 30 days
                if (startTypes.Contains(CriminalStartType.All) || startTypes.Contains(CriminalStartType.CaseTypes))
                {
                    await FetchCaseStylesAsync(driver).ConfigureAwait(false);
                }

                // get latest monthly records
                if (startTypes.Contains(CriminalStartType.All) || startTypes.Contains(CriminalStartType.Download))
                {
                    await FetchLastDownloadAsync(driver).ConfigureAwait(false);
                }
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }
        }

        private static async Task FetchLastDownloadAsync(IWebDriver driver)
        {
            using HarrisCriminalData obj = new();
            string result = null;
            await Task.Run(() => { result = obj.GetData(driver); }).ConfigureAwait(false);
            Debug.Assert(result != null);
            Debug.Assert(File.Exists(result));
        }

        private static async Task FetchCaseStylesAsync(IWebDriver driver)
        {
            const int interval = -5;
            const int cycleId = 10;
            DateTime MxDate = DateTime.Now.AddDays(-1).Date;
            DateTime MnDate = MxDate.AddDays(interval);
            List<KeyValuePair<DateTime, DateTime>> dtes = GetDateRange(interval, cycleId, MxDate, MnDate);
            using HarrisCriminalCaseStyle obj = new();
            List<HarrisCriminalStyleDto> result = new();
            foreach (KeyValuePair<DateTime, DateTime> dateRange in dtes)
            {
                List<HarrisCriminalStyleDto> records = await Task.Run(() =>
                {
                    return obj.GetCases(driver, dateRange.Key, dateRange.Value);
                }).ConfigureAwait(false);
                result.Append(records);
            }
        }

        private static List<KeyValuePair<DateTime, DateTime>> GetDateRange(int interval, int cycleId, DateTime MxDate, DateTime MnDate)
        {
            List<KeyValuePair<DateTime, DateTime>> dtes = new()
            {
                new KeyValuePair<DateTime, DateTime>(MnDate, MxDate)
            };
            while (dtes.Count < cycleId)
            {
                KeyValuePair<DateTime, DateTime> item = dtes.Last();
                dtes.Add(new KeyValuePair<DateTime, DateTime>(item.Key.AddDays(interval), item.Key));
            }

            return dtes;
        }

        private static IWebDriver GetDriver(bool headless = false)
        {
            const string title = "chrome";
            CultureInfo culture = CultureInfo.CurrentCulture;
            ChromeOlderProvider provider = new();
            IEnumerable<int> originalList = ListProcess(title).Split(',')
                .Select(x => Convert.ToInt32(x.Trim(), culture));

            IWebDriver driver = provider.GetWebDriver();
            if (headless)
            {
                string processIndexes = ListProcess(title);
                Debug.WriteLine(title);
                Debug.WriteLine(processIndexes);
                IEnumerable<int> list = processIndexes.Split(',')
                .Select(x => Convert.ToInt32(x.Trim(), culture))
                .Where(y => !originalList.Contains(y));
                Debug.WriteLine(string.Join(" - ", list));
                if (list.Any())
                {
                    EnumerateWindows.HideWindow(list.First());
                }
            }
            return driver;
        }

        private static void KillProcess(string processName)
        {
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }

        private static string ListProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            IEnumerable<int> handles = processes
                .Select(p => p.MainWindowHandle.ToInt32())
                .Where(x => x > 0);
            return string.Join(", ", handles);
        }
    }
}
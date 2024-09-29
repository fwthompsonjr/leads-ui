namespace legallead.records.search.Web
{
    using legallead.records.search.Dto;
    using legallead.records.search.Models;
    using legallead.records.search.Tools;
    using Microsoft.VisualStudio.Shell;
    using Newtonsoft.Json;
    using OpenQA.Selenium;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Xml;

    public class HarrisJpSubmitForm : ElementActionBase
    {
        private const string actionName = "harris-jp-submit-form";

        public override string ActionName => actionName;

        public List<PersonAddress> People { get; private set; } = [];

        [SuppressMessage("Usage", 
            "VSTHRD002:Avoid problematic synchronous waits", 
            Justification = "Async pattern failed to return proper result")]
        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            IWebDriver? driver = GetWeb;
            if (driver == null) { return; }
            string selector = item.Locator.Query;
            string form = item.Locator.Query.Replace("#", "");
            if (string.IsNullOrEmpty(selector))
            {
                return;
            }

            string[] getaction = [
                "var origin = document.location.origin;",
                $"var action = $('{selector}').attr('action');",
                "return ''.concat(origin, action);"
            ];
            string[] getvalues = [
                "var obj = {};",
                $"var formData = new FormData(document.getElementById('{form}'));",
                "formData.forEach((value, key) => obj[key] = value);",
                "return JSON.stringify(obj);"
            ];
            string getcourt = "return $('#court option:selected').text().trim();";
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            var action = Convert.ToString(jse.ExecuteScript(string.Join(Environment.NewLine, getaction)));
            var values = Convert.ToString(jse.ExecuteScript(string.Join(Environment.NewLine, getvalues))) ?? "{}";
            var court = Convert.ToString(jse.ExecuteScript(getcourt)) ?? " - ";
            var items = JsonConvert.DeserializeObject<Dictionary<string, string>>(values) ?? [];
            var keys = items.Keys.ToList();
            _ = items.TryGetValue("casetype", out string? extractType);
            _ = items.TryGetValue("fdate", out string? filingDt);
            if (string.IsNullOrEmpty(extractType) || !extractType.Contains("CRCIT")) extractType = "civil";
            else extractType = "criminal";

            var querystring = string.Join("", keys.Select(x =>
            {
                var id = keys.IndexOf(x);
                var prefix = id == 0 ? "?" : "&";
                return $"{prefix}{x}={HttpUtility.UrlEncode(items[x])}";
            }));
            var uri = string.Concat(action, querystring);
            var ms = new MemoryStream();
            var client = new HttpClient();
            var response = client.GetStreamAsync(uri).GetAwaiter().GetResult();
            response.CopyTo(ms);
            var contents = Encoding.UTF8.GetString(ms.ToArray());
            var doc = new XmlDocument();
            doc.LoadXml(contents);
            People = doc.MapFrom(extractType);
            People.ForEach(p =>
            {
                if (string.IsNullOrEmpty(p.Court) && !string.IsNullOrEmpty(court)) p.Court = court;
                if (string.IsNullOrEmpty(p.DateFiled) && !string.IsNullOrEmpty(filingDt)) p.DateFiled = filingDt;
            });
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}
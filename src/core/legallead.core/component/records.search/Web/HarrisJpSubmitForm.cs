namespace legallead.records.search.Web
{
    using legallead.records.search.Dto;
    using Newtonsoft.Json;
    using OpenQA.Selenium;
    using System.Text;
    using System.Threading;
    using System.Web;

    public class HarrisJpSubmitForm : ElementActionBase
    {
        private const string actionName = "harris-jp-submit-form";

        public override string ActionName => actionName;

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

            string[] getaction = new[] {
                "var origin = document.location.origin;",
                $"var action = $('{selector}').attr('action');",
                "return ''.concat(origin, action);"
            };
            string[] getvalues = new[] {
                "var obj = {};",
                $"var formData = new FormData(document.getElementById('{form}'));",
                "formData.forEach((value, key) => obj[key] = value);",
                "return JSON.stringify(obj);"
            };
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            var action = Convert.ToString(jse.ExecuteScript(string.Join(Environment.NewLine, getaction)));
            var values = Convert.ToString(jse.ExecuteScript(string.Join(Environment.NewLine, getvalues))) ?? "{}";
            var items = JsonConvert.DeserializeObject<Dictionary<string,string>>(values) ?? new();
            var keys = items.Keys.ToList();
            var querystring =string.Join("", keys.Select(x =>
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

            
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}
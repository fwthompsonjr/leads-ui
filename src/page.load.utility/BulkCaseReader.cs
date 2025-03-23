namespace page.load.utility
{
    using page.load.utility.Entities;
    using page.load.utility.Extensions;
    using Polly.Timeout;
    using Polly;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using SRC = OpenQA.Selenium.Cookie;
    using System.Net;
    using DST = System.Net.Cookie;
    using HtmlAgilityPack;

    public class BulkCaseReader(
        ReadOnlyCollection<SRC> settings, 
        List<CaseItemDto> items,
        BulkReadMessages response)
    {
        private readonly ReadOnlyCollection<SRC> cookies = settings;
        private readonly List<CaseItemDto> Workload = items;
        private readonly BulkReadMessages Log = response;
        public EventHandler<BulkReadMessages>? OnStatusUpdated;

        public object? Execute()
        {
            var list = new ConcurrentDictionary<int, CaseItemDtoMapper>();
            Workload.ForEach(x =>
            {
                list[Workload.IndexOf(x)] = new() { Dto = x };
            });
            var retries = 0;
            var count = Workload.Count;
            const int seconds = 15;
            while (list.Any(x => !x.Value.IsMapped()))
            {
                Workload.ForEach(c =>
                {
                    var id = Workload.IndexOf(c);
                    IterateWorkLoad(id, c, cookies, list, count);
                });
                var unresloved = list.Count(x => !x.Value.IsMapped());
                var currentDate = DateTime.Now;
                if (unresloved == 0) {
                    Log.Messages.Add($"{currentDate:G}: Processed {count - unresloved} items.");
                    OnStatusUpdated?.Invoke(this, Log);
                    break; 
                }
                Log.Messages.Add($"{currentDate:G}: Processed {count - unresloved} items.");
                Log.Messages.Add($"{currentDate:G}: Found {unresloved} items needing review.");
                Log.Messages.Add($"{currentDate:G}: Waiting {seconds:F2} seconds before retry.");
                OnStatusUpdated?.Invoke(this, Log);
                var wait = TimeSpan.FromSeconds(seconds);
                Thread.Sleep(wait);
                retries++;
            }
            Workload.Clear();
            var dtos = list.Select(x => x.Value.Dto ?? new());
            Workload.AddRange(dtos);
            // Cast ConcurrentDictionary to Dictionary before returning
            return Workload.ToJsonString();
        }

        private void IterateWorkLoad(
            int idx,
            CaseItemDto c,
            ReadOnlyCollection<SRC> cookies,
            ConcurrentDictionary<int, CaseItemDtoMapper> cases,
            int count)
        {
            var instance = cases[idx];
            if (instance.IsMapped()) return;
            if (idx % 5 == 0)
            {
                Thread.Sleep(500);
            }
            var content = GetContentWithPollyAsync(c.Href, cookies).GetAwaiter().GetResult();
            var readFailed = string.IsNullOrEmpty(content) || content.Equals("error");
            var currentDate = DateTime.Now;
            var msg = $"{currentDate:G}: Reading item {idx + 1} of {count}. Case {instance.Dto?.CaseNumber ?? "---"}";
            if (readFailed) msg += ". FAIL - Adding to retry";
            Log.Messages.Add(msg);
            OnStatusUpdated?.Invoke(this, Log);
            if (readFailed) return;
            var data = GetPageContent(content);
            instance.MappedContent = data;
            instance.Map();
        }

        private static async Task<string> GetContentWithPollyAsync(string href, ReadOnlyCollection<SRC> cookies)
        {
            var timeoutPolicy = Policy.TimeoutAsync(6, TimeoutStrategy.Pessimistic);
            var fallbackPolicy = Policy<string>
                .Handle<Exception>()
                .Or<TimeoutRejectedException>() // Handle timeout exceptions
                .FallbackAsync(async (cancellationToken) =>
                {
                    await Task.Run(() => { Thread.Sleep(TimeSpan.FromMinutes(1)); }, cancellationToken);
                    return string.Empty;
                });

            var policyWrap = timeoutPolicy.WrapAsync(fallbackPolicy);

            try
            {
                return await policyWrap.ExecuteAsync(async () =>
                {
                    return await GetPageAsync(href, cookies);
                });
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static async Task<string> GetPageAsync(string uri, IEnumerable<SRC> cookies)
        {
            var baseAddress = new Uri(uri);
            var cookieContainer = new CookieContainer();
            var cookieJar = cookies.ToList();
            var cookieCollection = new CookieCollection();
            foreach (var cookie in cookieJar)
            {
                if (string.IsNullOrEmpty(cookie.Value.Trim())) continue;
                cookieCollection.Add(new DST(cookie.Name, cookie.Value));
            }
            cookieContainer.Add(baseAddress, cookieCollection);
            var result = await GetRemotePageAsync(baseAddress, cookieContainer);
            return result;
        }


        private static async Task<string> GetRemotePageAsync(Uri baseAddress, CookieContainer cookieContainer)
        {
            try
            {
                using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromMilliseconds(3500) };
                var result = await client.GetAsync(baseAddress);
                if (result.IsSuccessStatusCode)
                {
                    var contents = await result.Content.ReadAsStringAsync();
                    return contents;
                }
                return "";
            }
            catch (Exception)
            {
                return "error";
            }
        }


        private static string? GetPageContent(string html)
        {
            const string pipe = "|";
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var obj = new TheAddress();
            var node = doc.DocumentNode;
            var caseStyle = string.Empty;
            var paragraphs = node.SelectNodes(HtmlSelectors.ParagraghTextPrimary);
            if (paragraphs == null) return obj.ToJsonString();
            if (paragraphs.Count > 0)
            {
                var target = paragraphs.FirstOrDefault(s => s.InnerText.Contains(pipe));
                if (target != null)
                {
                    var a = target.InnerText.IndexOf(pipe);
                    caseStyle = target.InnerText[(a + 1)..].Trim();
                }
            }
            obj.CaseHeader = caseStyle;
            var dv = node.SelectSingleNode(HtmlSelectors.DivCaseInformationBody);
            if (dv == null) { return obj.ToJsonString(); }
            var dvs = dv.SelectNodes(HtmlSelectors.Div);
            if (dvs == null || dvs.Count < 2) { return obj.ToJsonString(); }
            dv = dvs[2];
            if (string.IsNullOrEmpty(dv.InnerText)) return string.Empty;
            var arr = dv.InnerText.Trim().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 2) return string.Empty;
            var cs = arr[1].Trim(); // this is actually the case number
            obj.CaseNumber = cs;
            /* get plaintiff */
            var list = node.SelectNodes(HtmlSelectors.Span).ToList();
            var find = list.FindAll(a =>
            {
                var attr = a.GetAttributeValue("class", "null");
                if (attr == "null") { return false; }
                return attr == "text-primary";
            }).FindAll(b => b.InnerText.Trim() == "PLAINTIFF");
            if (find == null || find.Count == 0) { return obj.ToJsonString(); }
            var pln = find[0];
            var paragraph = Closest(pln, "p");
            if (paragraph == null) { return obj.ToJsonString(); }
            Console.WriteLine($"     - Reading case: {cs}");
            obj.Plaintiff = paragraph.InnerText.Replace("PLAINTIFF", "").Trim();
            /* get defendant address */
            var dvparty = node.SelectSingleNode(HtmlSelectors.DivPartyInformationBody);
            var partytypes = new List<string> { "RESPONDENT", "DEFENDANT" };
            if (dvparty == null) return obj.ToJsonString();
            var children = dvparty.SelectNodes(HtmlSelectors.Paragragh)?.ToList();
            if (children == null || children.Count == 0) { return obj.ToJsonString(); }
            var parties = children.FindAll(f =>
            {
                var found = false;
                var dvp = Closest(f, "div");
                if (dvp == null) { return false; }
                var txt = dvp?.InnerText?.Trim() ?? string.Empty;
                partytypes.ForEach(p => { if (txt.Contains(p)) { found = true; } });
                if (!found) { return false; }
                return f.InnerHtml.IndexOf("<span") < 0;
            }).ToList();
            if (parties.Count == 0) return obj.ToJsonString();
            var addr = parties[0].InnerText.Trim();
            addr = addr.Replace(Environment.NewLine, "|").Trim();
            while (addr.Contains("||")) { addr = addr.Replace("||", "|").Trim(); }
            obj.Address = addr;
            return obj.ToJsonString();
        }


        private static HtmlNode? Closest(HtmlNode node, string elementName)
        {
            if (node == null) { return null; }
            var parent = node.ParentNode;
            while (parent != null && !parent.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase))
            {
                parent = parent.ParentNode;
            }
            return parent;
        }

        private static class HtmlSelectors
        {
            public const string Div = "div";
            public const string DivCaseInformationBody = "//*[@id='divCaseInformation_body']";
            public const string DivPartyInformationBody = "//*[@id='divPartyInformation_body']";
            public const string Paragragh = "//p";
            public const string ParagraghTextPrimary = "//p[@class='text-primary']";
            public const string Span = "//span";
        }
    }
}

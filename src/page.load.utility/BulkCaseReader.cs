namespace page.load.utility
{
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using page.load.utility.Entities;
    using page.load.utility.Extensions;
    using page.load.utility.Interfaces;
    using Polly;
    using Polly.Timeout;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Net;
    using DST = System.Net.Cookie;
    using SRC = OpenQA.Selenium.Cookie;

    public class BulkCaseReader(
        ReadOnlyCollection<SRC> settings,
        List<CaseItemDto> items,
        BulkReadMessages response)
    {
        private readonly ReadOnlyCollection<SRC> cookies = settings;
        private readonly List<CaseItemDto> Workload = items;
        private readonly BulkReadMessages Log = response;
        private readonly List<string> FileDates = [.. items.Select(x => x.FileDate).Distinct()];
        private readonly IFetchDbAddress? AddressService = response.AddressSerice;
        private readonly int CountyId = response.CountyId;

        public EventHandler<BulkReadMessages>? OnStatusUpdated { get; set; }
        public EventHandler<BulkReadMessages>? OnStatusTimeOut { get; set; }
        public object? Execute()
        {
            var list = new ConcurrentDictionary<int, CaseItemDtoMapper>();
            Workload.ForEach(x =>
            {
                list[Workload.IndexOf(x)] = new() { Dto = x };
            });
            var retries = 0;
            var count = Workload.Count;
            Log.RecordCount = count;
            while (list.Any(x => !x.Value.IsMapped()))
            {
                Workload.ForEach(c =>
                {
                    var id = Workload.IndexOf(c);
                    IterateWorkLoad(id, c, cookies, list, count);
                });
                var unresloved = list.Count(x => !x.Value.IsMapped());
                var currentDate = GetCentralTime();
                if (unresloved == 0)
                {
                    Log.TotalProcessed = Log.RecordCount;
                    Log.Messages.Add($"{currentDate:G}| Processed {count - unresloved} items.");
                    OnStatusUpdated?.Invoke(this, Log);
                    break;
                }
                var unresolvedCount = Math.Round(Convert.ToDouble(unresloved) / Convert.ToDouble(count), 3) * 100;
                var delay = unresolvedCount > 10 ? Math.Min(Timings.UnresolvedRecordWaitMinimumSeconds * 3, 75) : Timings.UnresolvedRecordWaitMinimumSeconds;
                var isRetryNeeded = IsRetryNeeded(retries, Log.Messages, FileDates.Count);
                Log.RetryCount = isRetryNeeded ? retries + 1 : retries;
                Log.TotalProcessed = count - unresloved;
                Log.Messages.Add($"{currentDate:G}| Processed {count - unresloved} items.");
                Log.Messages.Add($"{currentDate:G}| Found {unresloved} items needing review.");
                Log.Messages.Add($"{currentDate:G}| Waiting {delay:F2} seconds before retry.");
                OnStatusUpdated?.Invoke(this, Log);
                if (!isRetryNeeded) break;
                var wait = TimeSpan.FromSeconds(delay);
                Thread.Sleep(wait);
                retries++;
            }
            Workload.Clear();
            var dtos = list.Select(x => x.Value.Dto ?? new());
            Workload.AddRange(dtos);
            var workload = Workload.ToJsonString() ?? Log.Workload;
            Log.Workload = workload;
            var dateCurrent = GetCentralTime();
            Log.Messages.Add($"{dateCurrent:G}| Processed completed {dtos.Count()} items.");
            OnStatusUpdated?.Invoke(this, Log);
            return workload;
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
            var currentDate = GetCentralTime();
            var msg = $"{currentDate:G}| Reading item {idx + 1} of {count}. Case {instance.Dto?.CaseNumber ?? "---"}";
            var address = AddressService?.FindAddress(CountyId, c.CaseNumber);
            if (address != null)
            {
                var jsaddress = JsonConvert.SerializeObject(address);
                instance.MappedContent = jsaddress;
                instance.Map();
                UpdateCaseStatus(cases, count, msg);
                return;
            }
            var content = GetContentWithPollyAsync(c.Href, cookies).GetAwaiter().GetResult();
            var readFailed = string.IsNullOrEmpty(content) || content.Equals("error");
            if (readFailed) msg += ". FAIL - Adding to retry";
            UpdateCaseStatus(cases, count, msg);
            if (readFailed)
            {
                int ms = (idx % 15 == 0) ? Timings.FailedResponseWaitMax : Timings.FailedResponseWaitMin;
                Thread.Sleep(ms);
                return;
            }
            var data = GetPageContent(content);
            instance.MappedContent = data;
            instance.Map();
        }

        private void UpdateCaseStatus(ConcurrentDictionary<int, CaseItemDtoMapper> cases, int count, string msg)
        {
            Log.TotalProcessed = count - cases.Count(x => !x.Value.IsMapped());
            Log.Messages.Add(msg);
            Log.Workload = cases.Select(x => x.Value.Dto ?? new()).ToJsonString() ?? string.Empty;
            OnStatusUpdated?.Invoke(this, Log);
        }

        private static async Task<string> GetContentWithPollyAsync(string href, ReadOnlyCollection<SRC> cookies)
        {
            var timeoutPolicy = Policy.TimeoutAsync(Timings.ProcessTimeInSeconds, TimeoutStrategy.Pessimistic);
            var fallbackPolicy = Policy<string>
                .Handle<Exception>()
                .Or<TimeoutRejectedException>() // Handle timeout exceptions
                .FallbackAsync(async (cancellationToken) =>
                {
                    await Task.Run(() => { Thread.Sleep(TimeSpan.FromMilliseconds(100)); }, cancellationToken);
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
                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromMilliseconds(Timings.HttpTimeInMilliSeconds) };
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

        private static DateTime GetCentralTime()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime centralTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, centralTimeZone);
            return centralTime;
        }

        private static class Timings
        {
            public const int ProcessTimeInSeconds = 4;
            public const int HttpTimeInMilliSeconds = 3000;
            public const int FailedResponseWaitMax = 200;
            public const int FailedResponseWaitMin = 75;
            public const int UnresolvedRecordWaitMinimumSeconds = 20;
            public const int TotalProcessTimeInMinutes = 45;
        }

        private bool IsRetryNeeded(int retryCount, List<string> messages, int dateCount)
        {
            if (retryCount > 4) return false; // max retries 5
            var dtCount = Math.Max(1, dateCount);
            var startTime = ParseDateFromMessage(messages[0]);
            var endTime = ParseDateFromMessage(messages[^1]);
            var elapsedMinutes = endTime.Subtract(startTime).TotalMinutes;
            var totalProcessAllowedTime = Timings.TotalProcessTimeInMinutes * dtCount;
            if (elapsedMinutes > totalProcessAllowedTime)
            {
                OnStatusTimeOut?.Invoke(this, Log);
                return false;
            }
            return true;
        }
        private static DateTime ParseDateFromMessage(string message)
        {
            var dte = message.Split('|')[0];
            return DateTime.Parse(dte, CultureInfo.CurrentCulture);
        }

    }
}

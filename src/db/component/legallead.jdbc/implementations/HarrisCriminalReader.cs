using HtmlAgilityPack;
using legallead.jdbc.interfaces;
using System.Net;

namespace legallead.jdbc.implementations
{
    internal class HarrisCriminalReader : IHarrisCriminalReader
    {
        protected string DatasetHtml { get; set; } = string.Empty;
        protected string DownloadUrl { get; set; } = string.Empty;
        protected byte[] DownloadContent { get; set; } = Array.Empty<byte>();
        private CookieContainer? CookieJar = null;
        public async Task<bool> Fetch(string datasetName)
        {
            const char sqte = (char)39;
            const string findTd = "//td[@style='vertical-align:top;']";
            const string findForm = "//*[@id='aspnetForm']";
            if (CookieJar == null) throw new InvalidOperationException();
            if (!Uri.TryCreate(datasetUri, UriKind.Absolute, out var uri)) return false;
            if (string.IsNullOrEmpty(DatasetHtml)) return false;
            var doc = new HtmlDocument();
            doc.LoadHtml(DatasetHtml);
            var collection = doc.DocumentNode.SelectNodes(findTd).ToList();
            if (collection.Count == 0) return false;
            var requested = collection.Find(x => x.InnerText.Contains(datasetName));
            if (requested == null) return false;
            var cells = requested.ParentNode.SelectNodes("td");
            if (cells.Count != 3) return false;
            var link = cells[2].SelectSingleNode("a");
            if (link == null) return false;
            var instruction = link.GetAttributeValue<string>("onclick", "");
            if (string.IsNullOrEmpty(instruction)) return false;
            if (!instruction.Contains(sqte)) return false;
            var form = doc.DocumentNode.SelectSingleNode(findForm);
            if (form == null) return false;
            var colInput = form.SelectNodes("input").ToList();
            if (colInput.Count < 5) return false;
            DownloadUrl = instruction.Split(sqte)[1];
            var formContent = new MultipartFormDataContent();
            colInput.ForEach(s =>
            {
                var name = s.GetAttributeValue<string>("name", "");
                var value = s.GetAttributeValue<string>("value", "");
                if (!name.Equals("hiddenDownloadFile"))
                {
                    formContent.Add(new StringContent(value), name);
                }
            });
            formContent.Add(new StringContent(DownloadUrl), "hiddenDownloadFile");
            var handler = new HttpClientHandler
            {
                CookieContainer = CookieJar
            };
            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var httpResponse = await client.PostAsync(uri, formContent);
            if (!httpResponse.IsSuccessStatusCode) return false;
            DownloadContent = await httpResponse.Content.ReadAsByteArrayAsync();
            return true;
        }

        public async Task<bool> Navigate()
        {
            if (!Uri.TryCreate(datasetUri, UriKind.Absolute, out var uri)) return false;
            CookieJar = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = CookieJar
            };
            using var client = new HttpClient(handler);
            var html = await client.GetStringAsync(uri);
            if (string.IsNullOrEmpty(html)) return false;
            DatasetHtml = html;
            return true;
        }

        public bool Translate()
        {
            throw new NotImplementedException();
        }

        public bool Upload()
        {
            throw new NotImplementedException();
        }

        private const string datasetUri = "https://www.hcdistrictclerk.com/Common/e-services/PublicDatasets.aspx";
    }
}

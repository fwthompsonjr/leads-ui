using git.project.reader.assets;
using git.project.reader.models;
using legallead.permissions.api.Models;
using System.Text;

namespace legallead.permissions.api.Services
{
    public class DownloadContentService : IDownloadContentService
    {
        public ReleaseAssetBo? GetAssetDetail(long releaseId, long assetId)
        {
            if (!ReleaseDetails.TryGetValue(releaseId, out var assetbo)) return null;
            if (!assetbo.IsAssetChecked) return null;
            var expected = assetbo.Assets.Find(x => x.Id == assetId);
            if (expected == null) return null;
            return assetbo;
        }
        public async Task<byte[]?> GetAssetsAsync(long releaseId, string assetName)
        {
            var response = await manager.DownloadAssetAsync(releaseId, assetName);
            return response;

        }

        public string GetContent(string page, ReleaseModel? model = null)
        {
            var pageId = page switch
            {
                _ => "download-blank"
            };
            var html = new StringBuilder(GetResourceText(pageId))
                .Replace("<!-- append head section -->", GetResourceText("download-css"))
                .Replace("<!-- append scripts section -->", GetResourceText("download-jscript")).ToString();
            if (model == null) return html;
            return GetTable(html, model);
        }

        public async Task<DownloadContentResponse> GetDownloadsAsync(string page)
        {
            var response = new DownloadContentResponse
            {
                Content = GetContent(page),
                CreationDate = DateTime.UtcNow,
            };
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response.Content);
            var dv = doc.DocumentNode.SelectSingleNode("//*[@id='dv-subcontent-invoice']");
            if (dv is null) return response;
            var children = dv.SelectNodes("div");
            if (children == null || children.Count == 0 || children[0].ChildNodes.Count < 4) return response;
            var child = children[0].ChildNodes[3];
            var releases = await manager.ListReleases();
            var tbl = GetTable(releases);
            child.InnerHtml = tbl;
            response.Models.Clear();
            response.Models.AddRange(releases);
            response.Content = doc.DocumentNode.OuterHtml;
            return response;
        }

        private static string GetTable(List<ReleaseModel> releases)
        {
            const string linkText = "    <td><a href='javascript:void(0)' class='link-primary' onclick=\"goToReleaseDetail('{0}');\">{0}</a></td>";
            var builder = new StringBuilder("<table style='margin-left: 60px; width: 85%' name='table-releases'>");
            builder.AppendLine("<colgroup>");
            builder.AppendLine("  <col name='index' style='width: 115px'>");
            builder.AppendLine("  <col />");
            builder.AppendLine("  <col name='create-date' style='width: 180px'>");
            builder.AppendLine("</colgroup>");
            builder.AppendLine("<thead>");
            builder.AppendLine("  <tr>");
            builder.AppendLine("    <td>Id</td>");
            builder.AppendLine("    <td>Name</td>");
            builder.AppendLine("    <td>Date</td>");
            builder.AppendLine("  </tr>");
            builder.AppendLine("</thead>");
            builder.AppendLine("<tbody>");
            releases.ForEach(r =>
            {

                builder.AppendLine("  <tr>");
                builder.AppendLine(string.Format(linkText, r.Id));
                builder.AppendLine($"    <td>{r.Name}</td>");
                builder.AppendLine($"    <td>{r.CreatedAt.DateTime:g}</td>");
                builder.AppendLine("  </tr>");
            });
            builder.AppendLine("</tbody>");
            builder.AppendLine("</table>");
            return builder.ToString();
        }

        private static string GetTable(string html, ReleaseModel model)
        {
            const string asset_place_holder = "<!-- assert insert -->";
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var dv = doc.DocumentNode.SelectSingleNode("//*[@id='dv-subcontent-invoice']");
            if (dv is null) return html;
            var children = dv.SelectNodes("div");
            if (children == null || children.Count == 0 || children[0].ChildNodes.Count < 4) return html;
            var child = children[0].ChildNodes[3];

            var builder = new StringBuilder("<table style='margin-left: 60px; width: 85%' name='table-releases'>");
            builder.AppendLine("<colgroup>");
            builder.AppendLine("  <col name='index' style='width: 115px'>");
            builder.AppendLine("  <col />");
            builder.AppendLine("  <col name='create-date' style='width: 180px'>");
            builder.AppendLine("</colgroup>");
            builder.AppendLine("<thead>");
            builder.AppendLine("  <tr style='display: none'>");
            builder.AppendLine("    <td>Id</td>");
            builder.AppendLine("    <td>Name</td>");
            builder.AppendLine("    <td>Date</td>");
            builder.AppendLine("  </tr>");
            builder.AppendLine("  <tr id='tr-download-status' style='display: none'>");
            builder.AppendLine("    <td colspan='3'>Download in progress ...</td>");
            builder.AppendLine("  </tr>");
            builder.AppendLine("</thead>");
            builder.AppendLine("<tbody>");

            builder.AppendLine("  <tr style='display: none'>");
            builder.AppendLine($"    <td>{model.Id}</td>");
            builder.AppendLine($"    <td>{model.Name}</td>");
            builder.AppendLine($"    <td>{model.CreatedAt.DateTime:g}</td>");
            builder.AppendLine("  </tr>");
            builder.AppendLine($"  {asset_place_holder}");
            var url = model.HtmlUrl;
            if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var _))
            {
                string data = DownloadDescription(url, model.Id);

                builder.AppendLine("  <tr>");
                builder.AppendLine($"    <td colspan='3' style='padding: 4px;'>");
                builder.AppendLine(data);
                builder.AppendLine($"    </td>");
                builder.AppendLine("  </tr>");
            }
            if (ReleaseDetails.TryGetValue(model.Id, out var assetbo) && (assetbo.Assets.Count == 0 && !assetbo.IsAssetChecked))
            {
                var assets = manager.ListAssets(model.Id).GetAwaiter().GetResult();
                if (assets.Count != 0)
                {
                    var converted = assets.Select(x =>
                    {
                        return new ReleaseAssetDto
                        {
                            Name = x.Name,
                            Id = x.Id,
                            DownloadUrl = x.DownloadUrl,
                        };
                    });
                    assetbo.Assets.AddRange(converted);
                }
                assetbo.IsAssetChecked = true;
            }
            if (ReleaseDetails.TryGetValue(model.Id, out var bo) && bo.Assets.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("<tr><td colspan='3'><h2>Assets</h2></td></tr>");
                sb.AppendLine("</tr>");
                bo.Assets.ForEach(a =>
                {
                    sb.AppendLine("<tr>");
                    sb.AppendLine($"  <td><a class='link-primary' href='javascript:void(0)' onclick=\"downloadAsset('{a.Id}')\">{a.Id}</a></td>");
                    sb.AppendLine($"  <td colspan='2'>{a.Name}</td>");
                    sb.AppendLine("</tr>");
                });
                builder.Replace(asset_place_holder, sb.ToString());
            }
            builder.AppendLine("</tbody>");
            builder.AppendLine("</table>");
            child.InnerHtml = builder.ToString();
            return doc.DocumentNode.OuterHtml;
        }

        private static string DownloadDescription(string uri, long index = 0)
        {
            if (index > 0 && ReleaseDetails.TryGetValue(index, out var assetbo))
            {
                return assetbo.Html;
            }
            var client = new HttpClient();
            var response = client.GetAsync(uri).Result;
            if (!response.IsSuccessStatusCode) return string.Empty;
            var content = response.Content.ReadAsStringAsync().Result ?? string.Empty;
            if (string.IsNullOrWhiteSpace(content)) return content;
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);
            var nde = doc.DocumentNode.SelectSingleNode("//*[@id=\"repo-content-pjax-container\"]/div/div/div/div[1]/div[3]");
            if (nde == null) return content;
            var notes = new StringBuilder(nde.InnerHtml);
            notes.Replace("<h4", "<h6");
            notes.Replace("</h4", "</h6");
            notes.Replace("<h3", "<h5");
            notes.Replace("</h3", "</h5");
            notes.Replace("<h2", "<h4");
            notes.Replace("</h2", "</h4");
            notes.Replace("<h1", "<h3");
            notes.Replace("</h1", "</h3");
            var bo = new ReleaseAssetBo { ReleaseId = index, Html = notes.ToString() };
            if (index != 0) ReleaseDetails.Add(index, bo);
            return notes.ToString();
        }
        private static string GetResourceText(string name)
        {
            var blank = Properties.Resources.ResourceManager.GetObject(name);
            if (blank is not byte[] data) return string.Empty;
            return Encoding.UTF8.GetString(data);
        }
        private static readonly ReleasesManager manager = new();
        private static readonly Dictionary<long, ReleaseAssetBo> ReleaseDetails = new();

    }
}

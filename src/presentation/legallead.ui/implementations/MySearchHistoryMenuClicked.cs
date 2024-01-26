using HtmlAgilityPack;

namespace legallead.ui.implementations
{
    internal class MySearchHistoryMenuClicked : MySearchMenuClicked
    {
        protected virtual int TargetIndex => 1;
        protected override string Transform(string text)
        {
            const string cls = "class";
            const string actv = "active";
            const string none = "d-none";
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            var parent = doc.DocumentNode;
            for (int i = 0; i < navlinks.Length; i++)
            {
                var nv = $"//*[@id ='{navlinks[i]}']";
                var vw = $"//*[@id ='{viewnames[i]}']";
                var nodeLink = parent.SelectSingleNode(nv);
                var viewLink = parent.SelectSingleNode(vw);
                if (nodeLink == null || viewLink == null) continue;
                var ndes = nodeLink.Attributes[cls].Value.Split(' ').ToList();
                var views = viewLink.Attributes[cls].Value.Split(' ').ToList();
                if (i == TargetIndex && !ndes.Contains(actv)) { ndes.Add(actv); }
                if (i != TargetIndex && ndes.Contains(actv)) { ndes.RemoveAll(a => a.Equals(actv)); }
                if (i == TargetIndex && !views.Contains(actv)) { views.Add(actv); }
                if (i != TargetIndex && views.Contains(actv)) { views.RemoveAll(a => a.Equals(actv)); }
                if (i != TargetIndex && !views.Contains(none)) { views.Add(none); }
                if (i == TargetIndex && views.Contains(none)) { views.RemoveAll(a => a.Equals(none)); }
                nodeLink.Attributes[cls].Value = string.Join(' ', ndes);
                viewLink.Attributes[cls].Value = string.Join(' ', views);

            }
            return doc.DocumentNode.OuterHtml;
        }
        private static readonly string[] navlinks = [
            "nvlink-subcontent-search",
            "nvlink-subcontent-search-history",
            "nvlink-subcontent-search-purchases"];

        private static readonly string[] viewnames = [
            "dv-search-container",
            "dv-subcontent-history",
            "dv-subcontent-purchases"];
    }
}

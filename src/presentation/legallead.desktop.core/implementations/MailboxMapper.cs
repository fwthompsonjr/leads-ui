using HtmlAgilityPack;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using Newtonsoft.Json;
using System;
using System.Text;

namespace legallead.desktop.implementations
{
    internal class MailboxMapper : UserProfileMapper, IUserMailboxMapper
    {
        public string Substitute(IMailPersistence? persistence, string source)
        {
            const string tareajson = "<!-- include: js mail collection -->";
            const string encodedtareajson = "&lt;!-- include: html current view --&gt;";
            const string tareacurrentview = "<!-- include: html current view -->";
            const string encodedtareacurrentview = "&lt;!-- include: js mail collection --&gt;";
            const string findList = "//*[@id=\"dv-mail-item-list\"]";
            var document = GetDocument(source);
            if (document == null) return source;
            var data = persistence?.Fetch() ?? string.Empty;
            var list = GetData(persistence, data);
            var content = list.Count == 0 ? 
                string.Empty : 
                persistence?.Fetch(list[0].Id ?? string.Empty) ?? string.Empty;
            if (list.Count > 0)
            {
                var listElement = document.DocumentNode.SelectSingleNode(findList);
                if (listElement != null)
                {
                    list.ForEach(item =>
                    {
                        var position = list.IndexOf(item);
                        var child = GetListItem(document, item, position);
                        if (child != null) { listElement.AppendChild(child); }
                    });
                }
            }
            

            var builder = new StringBuilder(document.DocumentNode.OuterHtml);
            var replacements = new[]
            {
                new { find = tareajson, replace = data},
                new { find = encodedtareajson, replace = data},
                new { find = tareacurrentview, replace = content},
                new { find = encodedtareacurrentview, replace = content},
            };
            var doubleLine = string.Concat(Environment.NewLine, Environment.NewLine);
            foreach (var item in replacements)
            {
                var token = string.Concat(doubleLine, item.replace, doubleLine);
                builder.Replace(item.find, token);
            }
            return builder.ToString();
        }

        private static List<MailStorageItem> GetData(IMailPersistence? persistence, string messages)
        {
            if (persistence == null || string.IsNullOrEmpty(messages)) return new();
            return ObjectExtensions.TryGet<List<MailStorageItem>>(messages);
        }
        private static HtmlNode GetListItem(HtmlDocument document, MailStorageItem item, int index)
        {
            var element = document.CreateElement("a");
            var active = index == 0 ? " active " : string.Empty;
            element.Attributes.Append("name", "link-mail-items-template");
            element.Attributes.Append("href", $"javascript:fetch_item({index})");
            element.Attributes.Append("data-item-index", "1");
            element.Attributes.Append("class", $"{active}list-group-item list-group-item-action".Trim());
            var header = GetHeaderNode(document, item);
            var detail = GetDetailNode(document, item);
            element.AppendChild(header);
            element.AppendChild(detail);
            return element;
        }

        private static HtmlNode GetHeaderNode(HtmlDocument document, MailStorageItem item)
        {
            var element = document.CreateElement("div");
            var subject = document.CreateElement("h5");
            var createDate = document.CreateElement("small");
            element.Attributes.Append("name", "item-header");
            element.Attributes.Append("class", "d-flex w-100 justify-content-between");
            subject.Attributes.Append("name", "item-subject");
            subject.Attributes.Append("class", $"mb-1");
            subject.InnerHtml = item.Subject ?? " - ";
            createDate.Attributes.Append("name", "item-create-date");
            createDate.InnerHtml = item.CreateDate ?? " - ";
            element.AppendChild(subject);
            element.AppendChild(createDate);
            return element;

        }

        private static HtmlNode GetDetailNode(HtmlDocument document, MailStorageItem item)
        {
            var element = document.CreateElement("div");
            var toDv = document.CreateElement("div");
            var fromDv = document.CreateElement("div");
            var itemDv = document.CreateElement("span");

            element.Attributes.Append("name", "item-detail");
            element.Attributes.Append("class", "row");

            // to addresss
            toDv.Attributes.Append("name", "item-address-to");
            toDv.Attributes.Append("class", "col-6 text-start");
            toDv.InnerHtml = $"To: {item.ToAddress ?? string.Empty}";
            // from address
            fromDv.Attributes.Append("name", "item-address-to");
            fromDv.Attributes.Append("class", "col-6 text-start");
            fromDv.InnerHtml = $"From: {item.FromAddress ?? string.Empty}";

            itemDv.Attributes.Append("name", "item-index");
            itemDv.Attributes.Append("class", "d-none");
            itemDv.InnerHtml = item.Id ?? " - ";

            element.AppendChild(toDv);
            element.AppendChild(fromDv);
            element.AppendChild(itemDv);

            return element;

        }
    }
}

﻿using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace legallead.desktop.implementations
{
    internal class ContentHtmlNames : IContentHtmlNames
    {
        private readonly ICopyrightBuilder? _copyrightBuilder;

        public ContentHtmlNames()
        {
            _copyrightBuilder = DesktopCoreServiceProvider.Provider.GetService<ICopyrightBuilder>();
        }

        public List<ContentHtml> ContentNames => _contents;

        public List<string> Names => _names ??= GetNames();
        public static List<ContentReplacementItem> ContentReplacements => contentReplacementItems ??= GetContentReplacements();

        public bool IsValid(string name)
        {
            return Names.Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        public ContentHtml? GetContent(string name)
        {
            const char minus = '-';
            if (!IsValid(name)) return null;
            var item = ContentNames
                    .Where(w => w.Name.Contains(minus))
                    .FirstOrDefault(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            if (item == null) return null;
            if (string.IsNullOrEmpty(item.Content))
            {
                MapResourceContent(item);
            }
            item = TransformFooterCopyRight(item);
            return item;
        }

        private ContentHtml TransformFooterCopyRight(ContentHtml item)
        {
            if (_copyrightBuilder == null) { return item; }
            var content = item.Content;
            var hasFooter = content.Contains(HtmCommonFooterCopyRight);
            if (!hasFooter) { return item; }
            var copy = _copyrightBuilder.GetCopyright();
            var text = string.Format(HtmCommonFooterCopyRight, copy);
            content = content.Replace(HtmCommonFooterCopyRight, text);
            item.Content = content;
            return item;
        }

        public Stream GetContentStream(string name)
        {
            var item = GetContent(name);
            if (item == null || string.IsNullOrEmpty(item.Content))
                return new System.IO.MemoryStream();
            return GenerateStreamFromString(item.Content);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private List<string>? _names;

        private static List<string> GetNames()
        {
            const char minus = '-';
            var names = _contents.Select(x => x.Name.Split(minus)[0].ToLower()).ToList();
            return names;
        }

        internal static string CommonReplacement(string? source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            var keys = Replacements.Keys;
            foreach (var key in from key in keys
                                where source.Contains(key)
                                select key)
            {
                source = source.Replace(key, Replacements[key]);
            }
            return source;
        }

        [ExcludeFromCodeCoverage]
        private static void MapResourceContent(ContentHtml? item)
        {
            if (item == null) return;
            if (!string.IsNullOrEmpty(item.Content)) return;
            var manager = Properties.Resources.ResourceManager;
            var resourceText = CommonReplacement(manager.GetString(item.Name));
            item.Content = resourceText;
        }

        private static string GetBaseCssScript()
        {
            var basecsstext = Properties.Resources.base_css;
            var builder = new StringBuilder("<style name=\"base-css\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</style>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static string GetLoginInclude()
        {
            var text = Properties.Resources.homelogin_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetRegistrationInclude()
        {
            var text = Properties.Resources.homeregister_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetWelcomeInclude()
        {
            var text = Properties.Resources.homewelcome_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetCommonCefInclude()
        {
            var text = Properties.Resources.commoncefhandler_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetCommonFooterInclude()
        {
            var text = Properties.Resources.commonfooter_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetCommonHeaderInclude()
        {
            var text = Properties.Resources.commonheadings_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetBootstrapCssScript()
        {
            var basecsstext = Properties.Resources.homelogin_html;
            var builder = new StringBuilder("<style name=\"base-css\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</style>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static string GetHomeValidationScript()
        {
            var basecsstext = Properties.Resources.homevalidation_js;
            var builder = new StringBuilder("<script name=\"home-form-validation\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</script>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static readonly List<ContentHtml> _contents = new()
        {
            new() { Index = -1, Name = "test"},
            new() { Index = 0, Name = "blank-html"},
            new() { Index = 10, Name = "base-css"},
            new() { Index = 100, Name = "introduction-html"},
            new() { Index = 110, Name = "home-html"},
            new() { Index = 110, Name = "homelogin-html"},
            new() { Index = 200, Name = "errorbox-css"},
            new() { Index = 300, Name = "myaccount-html"},
            new() { Index = 310, Name = "myaccounthome-html"},
            new() { Index = 315, Name = "myaccountpermissions-html"},
            new() { Index = 320, Name = "myaccountprofile-html"},
            new() { Index = 400, Name = "mysearch-html"},
        };

        private const string CssBaseLink = "<link rel=\"stylesheet\" name=\"base\" href=\"css/base.css\" />";
        private const string CssBootStrapLink = "<link rel=\"stylesheet\" href=\"bootstrap.min.css\" />";
        private const string CssErrorBox = "<link rel=\"stylesheet\" name=\"errorbox\" href=\"css/error.css\">";
        private const string CssMyAccountInclude = "<!-- style: my-account-custom-css -->";
        private const string CssMyAccountSubContent = "<!-- style: my-account-subcontent-css -->";
        private const string JsCommonCefHandler = "<!-- script: common-cef-handler -->";
        private const string HtmAccountHomeInclude = "<p>My Account</p>";
        private const string HtmAccountPasswordInclude = "<p>My Password</p>";
        private const string HtmAccountProfileInclude = "<p>My Profile</p>";
        private const string HtmAccountPermissionsInclude = "<p>My Permissions</p>";
        private const string HtmCommonFooter = "<!-- block: common-footer -->";
        private const string HtmCommonFooterCopyRight = "<span id=\"footer-copy-span\">{0}</span>";
        private const string HtmCommonHeading = "<!-- block: common-headings -->";
        private const string HtmCommonMastHead = "<!-- block: common-mast-heading -->";
        private const string HtmLoginInclude = "<p>Login form</p>";
        private const string HtmWelcomeInclude = "<p>Welcome form</p>";
        private const string HtmRegistrationInclude = "<p>Registration form</p>";
        private const string JsCommonReload = "/* js-include-common-reload */";
        private const string JsCommonClientInclude = "<!-- script: common-client-include -->";
        private const string JsHomeValidation = "<!-- script: home-form-validation -->";
        private const string JsMyAccountNavigation = "<!-- script: my-account-navigation -->";
        private const string JsMyAccountProfile = "<!-- script: my-account-profile-valid -->";
        private const string JsMyAccountPermissions = "/* inject: permissions-validation script */";

        private static readonly Dictionary<string, string> Replacements = new() {
            { CssBaseLink, GetBaseCssScript() },
            { CssBootStrapLink, GetBootstrapCssScript() },
            { CssErrorBox, Properties.Resources.errorbox_css },
            { CssMyAccountInclude, Properties.Resources.myaccount_css },
            { CssMyAccountSubContent, Properties.Resources.subcontent_css },
            { HtmAccountHomeInclude, Properties.Resources.myaccounthome_html },
            { HtmAccountPasswordInclude, Properties.Resources.myaccount_password_html },
            { HtmAccountProfileInclude, Properties.Resources.myaccountprofile_html },
            { HtmAccountPermissionsInclude, Properties.Resources.myaccountpermissions_html },
            { HtmLoginInclude, GetLoginInclude() },
            { HtmRegistrationInclude, GetRegistrationInclude() },
            { JsHomeValidation, GetHomeValidationScript() },
            { JsCommonCefHandler, GetCommonCefInclude() },
            { JsMyAccountNavigation, Properties.Resources.myaccount_script_js },
            { JsMyAccountProfile, Properties.Resources.myaccount_profile_validation_js },
            { HtmCommonFooter, GetCommonFooterInclude() },
            { HtmCommonHeading, GetCommonHeaderInclude() },
            { HtmWelcomeInclude, GetWelcomeInclude() },
            { HtmCommonMastHead, Properties.Resources.common_mast_head_html },
            { JsCommonReload, Properties.Resources.commonreload_js },
            { JsCommonClientInclude, Properties.Resources.commonclientinjection_js },
            { JsMyAccountPermissions, Properties.Resources.myaccount_permissions_validation_js },
        };

        private static List<ContentReplacementItem>? contentReplacementItems = null;

        private static List<ContentReplacementItem> GetContentReplacements()
        {
            var list = new List<ContentReplacementItem>();
            var keys = Replacements.Keys.ToList();
            keys.ForEach(k =>
            {
                list.Add(new ContentReplacementItem { Key = k, Value = Replacements[k] });
            });
            return list;
        }
    }
}
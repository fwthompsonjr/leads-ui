using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;

namespace legallead.desktop.implementations
{
    internal class UserProfileMapper : IUserProfileMapper
    {
        public async Task<string> Map(IPermissionApi api, UserBo user, string source)
        {
            var profile = await GetProfile(api, user);
            if (profile == null) return source;

            var document = GetDocument(source);
            if (document == null) return source;
            var replacements = new[]
            {
                new { node = "", find = "//*[@id=\"tbx-profile-first-name\"]", replace = GetProfileItem(profile, "Name", "First")},
                new { node = "", find = "//*[@id=\"tbx-profile-last-name\"]", replace = GetProfileItem(profile, "Name", "Last")},
                new { node = "", find = "//*[@id=\"tbx-profile-company\"]", replace = GetProfileItem(profile, "Name", "Company")},
                new { node = "textarea", find = "//*[@id=\"tbx-profile-mailing-address\"]", replace = GetProfileItem(profile, "Address", "Mailing")},
                new { node = "textarea", find = "//*[@id=\"tbx-profile-billing-address\"]", replace = GetProfileItem(profile, "Address", "Billing")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-01\"]", replace = GetProfileItem(profile, "Email", "Personal")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-02\"]", replace = GetProfileItem(profile, "Email", "Business")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-03\"]", replace = GetProfileItem(profile, "Email", "Other")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-01\"]", replace = GetProfileItem(profile, "Phone", "Personal")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-02\"]", replace = GetProfileItem(profile, "Phone", "Business")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-03\"]", replace = GetProfileItem(profile, "Phone", "Other")},
            };
            foreach (var item in replacements)
            {
                var element = document.DocumentNode.SelectSingleNode(item.find);
                if (element == null) continue;
                if (string.IsNullOrEmpty(item.node))
                {
                    element.SetAttributeValue("value", item.replace);
                }
                else
                {
                    element.InnerHtml = item.replace;
                }
            }
            return document.DocumentNode.OuterHtml;
        }

        private static HtmlDocument? GetDocument(string source)
        {
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(source);
                return document;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<List<ContactProfileItem>?> GetProfile(IPermissionApi api, UserBo user)
        {
            const string landing = "get-contact-detail";

            var payload = new { ResponseType = string.Empty };
            var response = await api.Post(landing, payload, user);
            if (response.StatusCode != 200) return null;
            var data = ObjectExtensions.TryGet<List<ContactProfileResponse>>(response.Message);
            if (data == null || !data.Any()) return null;
            var list = new List<ContactProfileItem>();
            var address = data.ToList().Find(a => a.ResponseType.Equals("Address"))?.Message;
            var names = data.ToList().Find(a => a.ResponseType.Equals("Name"))?.Message;
            var emails = data.ToList().Find(a => a.ResponseType.Equals("Email"))?.Message;
            var phones = data.ToList().Find(a => a.ResponseType.Equals("Phone"))?.Message;
            if (!string.IsNullOrEmpty(address))
            {
                var t1 = ObjectExtensions.TryGet<List<ContactAddress>>(address);
                t1?.ForEach(t => list.Add(t.ToItem()));
            }
            if (!string.IsNullOrEmpty(names))
            {
                var t2 = ObjectExtensions.TryGet<List<ContactName>>(names);
                t2?.ForEach(t => list.Add(t.ToItem()));
            }
            if (!string.IsNullOrEmpty(emails))
            {
                var t3 = ObjectExtensions.TryGet<List<ContactEmail>>(emails);
                t3?.ForEach(t => list.Add(t.ToItem()));
            }
            if (!string.IsNullOrEmpty(phones))
            {
                var t4 = ObjectExtensions.TryGet<List<ContactPhone>>(phones);
                t4?.ForEach(t => list.Add(t.ToItem()));
            }
            return list;
        }

        private static string GetProfileItem(List<ContactProfileItem> profile, string category, string code)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            var item = profile.Find(x => x.Category.Equals(category, comparison) &&
                x.Code.Equals(code, comparison));
            if (item == null) return string.Empty;
            return item.Data.Trim();
        }
    }
}
using HtmlAgilityPack;
using legallead.email.interfaces;
using legallead.email.models;
using legallead.email.transforms;
using legallead.email.utility;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Net.Mime;

namespace legallead.email.services
{
    internal class MailMessageService
    {
        private readonly IUserSettingInfrastructure _userDb;
        private readonly ISettingsService _settingsService;
        private readonly IHtmlTransformService _htmlService;
        private readonly IHtmlBeautifyService _beautifyService;
        public MailMessageService(
            ISettingsService settings,
            IUserSettingInfrastructure infrastructure,
            IHtmlTransformService transformService,
            IHtmlBeautifyService beautifyService)
        {
            _settingsService = settings;
            _userDb = infrastructure;
            _htmlService = transformService;
            _beautifyService = beautifyService;
            InitializeMessage();
        }

        public MailMessage? Message { get; private set; }

        public MailAddress? FromAddress { get; private set; }
        public MailAddress? ToAddress { get; private set; }
        public List<MailAddress>? CopyAddress { get; private set; }
        public string TemplateType { get; private set; } = "Legal Lead Email";
        public string? BodyHtml { get; private set; }
        public string? UserId { get; private set; }
        internal IUserSettingInfrastructure SettingsDb => _userDb;

        [ExcludeFromCodeCoverage(Justification = "Method is tested from pass-thru calls covering all common scenarios")]
        public bool CanSend()
        {
            if (Message == null) return false;
            if (FromAddress == null) return false;
            if (string.IsNullOrEmpty(Message.Subject)) return false;
            if (Message.From == null) return false;
            if (Message.To.Count == 0) return false;
            return !string.IsNullOrEmpty(Message.Body);
        }


        public MailMessageService With(TemplateNames template, string? userId, string email = "")
        {
            InitializeMessage();
            var hasId = Guid.TryParse(userId, out var guid);
            if (hasId) { _ = With(guid); }
            else { _ = With(email); }

            _ = EmailSubjects.TryGetValue(template, out var subject);
            subject ??= "Legal Lead Email";
            TemplateType = subject;
            if (Message != null) { Message.Subject = subject; }
            var service = _htmlService;
            if (service == null) return this;
            var query = hasId ?
                new UserSettingQuery { Id = userId } :
                new UserSettingQuery { Email = email };
            var html = service.GetHtmlTemplate(query, template.ToString()).GetAwaiter().GetResult();
            BodyHtml = SubstituteTitle(subject, html);
            if (Message != null && !string.IsNullOrEmpty(html))
            {
                var body = _beautifyService.BeautifyHTML(html);
                var doc = new HtmlDocument();
                doc.LoadHtml(body);

                Message.Body = body;
                Message.IsBodyHtml = true;
                ContentType mimeType = new("text/html");
                AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
                Message.AlternateViews.Add(alternate);
            }
            return this;
        }

        protected MailMessageService With(string userEmail)
        {
            var query = new UserSettingQuery { Email = userEmail };
            if (!query.IsValid) return this;
            var response = _userDb.GetSettings(query).GetAwaiter().GetResult();
            MapUserAddresses(response);
            return this;
        }

        protected MailMessageService With(Guid userId)
        {
            var query = new UserSettingQuery { Id = userId.ToString("D") };
            var response = _userDb.GetSettings(query).GetAwaiter().GetResult();
            MapUserAddresses(response);
            return this;
        }

        [ExcludeFromCodeCoverage(Justification = "Private class member tested fully from public method.")]
        private void InitializeMessage()
        {
            try
            {
                var from = _settingsService.GetSettings.Settings.From;
                var copyAdministrator = _settingsService.GetSettings.CopyToAdmin;
                FromAddress = new MailAddress(from.Email, from.DisplayName);
                Message = new MailMessage
                {
                    From = FromAddress
                };
                if (copyAdministrator)
                {
                    Message.Bcc.Add(FromAddress);
                }
            }
            catch (Exception)
            {
                Message = null;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private class member tested fully from public method.")]
        private void MapUserAddresses(List<UserEmailSettingBo>? settings)
        {
            var id = settings?.Find(x => !string.IsNullOrEmpty(x.Id))?.Id;
            if (id != null) { UserId = id; }
            ToAddress = GetToMailAddress(settings);
            CopyAddress = GetCcMailAddresses(settings);
            if (Message != null && ToAddress != null)
            {
                Message.To.Clear();
                Message.To.Add(ToAddress);
            }
            if (Message != null && CopyAddress != null)
            {
                Message.CC.Clear();
                CopyAddress.ForEach(Message.CC.Add);
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private class member tested fully from public method.")]
        private static string? SubstituteTitle(string subject, string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return html;
            const string query = "//h2[@name='span-sub-heading']";
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var node = doc.DocumentNode.SelectSingleNode(query);
                if (node == null) return html;
                node.InnerHtml = subject;
                return doc.DocumentNode.OuterHtml;
            }
            catch (Exception)
            {
                return html;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private class member tested fully from public method.")]
        private static MailAddress? GetToMailAddress(List<UserEmailSettingBo>? settings)
        {
            if (settings == null || settings.Count == 0) return null;
            var email = settings.Where(w => !string.IsNullOrEmpty(w.Email)).Distinct().FirstOrDefault()?.Email;
            if (string.IsNullOrWhiteSpace(email) || !IsValid(email)) return null;
            var firstName = settings.Find(s => (s.KeyName ?? "").Equals("First Name", StringComparison.OrdinalIgnoreCase))?.KeyValue;
            var lastName = settings.Find(s => (s.KeyName ?? "").Equals("Last Name", StringComparison.OrdinalIgnoreCase))?.KeyValue;
            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName)) return new MailAddress(email);
            var displayName = $"{firstName ?? string.Empty} {lastName ?? string.Empty}".Trim();
            return new MailAddress(email, displayName);
        }

        [ExcludeFromCodeCoverage(Justification = "Private class member tested fully from public method.")]
        private static List<MailAddress>? GetCcMailAddresses(List<UserEmailSettingBo>? settings)
        {
            if (settings == null || settings.Count == 0) return null;
            var emails = settings.FindAll(s =>
            {
                if (string.IsNullOrEmpty(s.KeyValue) || !IsValid(s.KeyValue)) return false;
                return (s.KeyName ?? "").StartsWith("Email ", StringComparison.OrdinalIgnoreCase);
            });
            if (emails.Count == 0) return null;
            var email = settings.Where(w => !string.IsNullOrEmpty(w.Email)).Distinct().FirstOrDefault()?.Email;
            var addresses = emails.Select(s => s.KeyValue ?? "").Distinct().ToList();
            addresses.RemoveAll(x => x.Equals(email, StringComparison.OrdinalIgnoreCase));
            return addresses.Select(s => new MailAddress(s)).ToList();
        }


        private static bool IsValid(string email)
        {
            var valid = true;

            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }

        private static readonly Dictionary<TemplateNames, string> EmailSubjects = new()
        {
            { TemplateNames.RegistrationCompleted, "Account Registration Completed" }
        };
    }
}

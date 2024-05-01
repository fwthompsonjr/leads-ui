using HtmlAgilityPack;
using legallead.email.models;
using legallead.email.services;
using legallead.email.utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

namespace legallead.email.actions
{
    internal class SearchPaymentCompleted(
        MailMessageService messaging,
        ISmtpService smtp) : BaseEmailAction(messaging, smtp)
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.Result is not ContentResult htmlResult) return;
            var html = htmlResult.Content;
            if (!IsHtmlValid(html)) return;
            var email = GetEmailAddress(html);
            var account = _mailMessageService.SettingsDb.GetUserByEmail(email).GetAwaiter().GetResult();
            if (!IsAccountValid(account)) return;
            
            _mailMessageService.With(TemplateNames.SearchPaymentCompleted, account?.Id);
            if (string.IsNullOrEmpty(_mailMessageService.UserId)) return;
            var id = _mailMessageService.UserId;
            var message = _mailMessageService.Message;
            if (!CanSendMessage(id, message)) return;
            message = ApplyTransform(message, html);
            _smtpService.Send(message, id);
        }
        private static MailMessage ApplyTransformProcess(MailMessage message, string html)
        {
            var messageDoc = new HtmlDocument();
            var doc = new HtmlDocument();

            doc.LoadHtml(html);
            messageDoc.LoadHtml(message.Body);
            var documentNode = doc.DocumentNode;
            var bodyNode = messageDoc.DocumentNode;
            if (documentNode == null || bodyNode == null) return message;
            findTokens.ForEach(token =>
            {
                var id = findTokens.IndexOf(token);
                var node = documentNode.SelectSingleNode(token);
                if (node != null)
                {
                    var query = targetTokens[id];
                    var target = bodyNode.SelectSingleNode(query);
                    if (target != null)
                    {
                        target.InnerHtml = node.InnerText;
                    }
                }
            });
            return message;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static string GetEmailAddress(string html)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var documentNode = doc.DocumentNode;
                if (documentNode == null) return string.Empty;
                var token = findTokens[0];
                var node = documentNode.SelectSingleNode(token);
                if (node == null) return string.Empty;
                return node.InnerText.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }


        [ExcludeFromCodeCoverage(Justification = "Wrapper method tested from public member.")]
        private static MailMessage? ApplyTransform(MailMessage? message, string html)
        {
            try
            {
                if (message == null) return null;
                return ApplyTransformProcess(message, html);
            }
            catch
            {
                return message;
            }

        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private bool CanSendMessage(string? id, MailMessage? message)
        {
            if (string.IsNullOrEmpty(id) || message == null || !CanSend()) return false;
            return true;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static bool IsHtmlValid(string html)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var documentNode = doc.DocumentNode;
                if (documentNode == null) return false;
                var found = 0;
                findTokens.ForEach(token =>
                {
                    var node = documentNode.SelectSingleNode(token);
                    if (node != null) found++;
                });
                return found == findTokens.Count;
            }
            catch
            {
                return false;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static bool IsAccountValid(UserAccountByEmailBo? account)
        {
            if (account == null || string.IsNullOrEmpty(account.Id)) return false;
            return true;
        }


        private static readonly List<string> findTokens = [
            "//span[@name='account-user-name']",
            "//span[@name='account-user-email']",
            "//div[@name='payment-details-payment-date']",
            "//div[@name='payment-details-payment-product']",
            "//div[@name='payment-details-payment-amount']",
            "//div[@name='payment-details-reference-id']",
        ];

        private static readonly List<string> targetTokens = [
            "//span[@name='search-payment-complete-user-name']",
            "//span[@name='search-payment-complete-email']",
            "//span[@name='search-payment-complete-payment-date']",
            "//span[@name='search-payment-complete-product']",
            "//span[@name='search-payment-complete-amount']",
            "//span[@name='search-payment-complete-reference-id']",
        ];

    }
}

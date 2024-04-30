using HtmlAgilityPack;
using legallead.email.services;
using legallead.email.utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Net.Mail;

namespace legallead.email.actions
{
    internal class SearchPaymentCompleted(
        MailMessageService messaging,
        ISmtpService smtp) : BaseEmailAction(messaging, smtp)
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Task.Run(() =>
            {
                if (context.Result is not ContentResult htmlResult) return;
                var html = htmlResult.Content;
                if (IsHtmlValid(html)) return;
                var email = GetEmailAddress(html);
                _mailMessageService.With(TemplateNames.SearchPaymentCompleted, "", email);
                var id = _mailMessageService.UserId;
                var message = _mailMessageService.Message;
                if (string.IsNullOrEmpty(id) || message == null || !CanSend()) return;
                message = ApplyTransform(message, html);
                _smtpService.Send(message, id);
            });
        }

        private static MailMessage ApplyTransform(MailMessage message, string html)
        {
            Debug.WriteLine(html);
            return message;
        }

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

        private static readonly List<string> findTokens = [
            "//span[@name='account-user-name']",
            "//span[@name='account-user-email']",
            "//div[@name='payment-details-payment-date']",
            "//div[@name='payment-details-payment-product']",
            "//div[@name='payment-details-payment-amount']",
            "//div[@name='payment-details-reference-id']",
        ];

    }
}

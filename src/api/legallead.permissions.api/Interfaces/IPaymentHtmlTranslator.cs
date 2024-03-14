using legallead.jdbc.entities;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IPaymentHtmlTranslator
    {
        Task<DownloadResponse> GetDownload(PaymentSessionDto dto);
        Task<bool> IsRequestDownloadedAndPaid(PaymentSessionDto? dto);
        Task<bool> IsRequestValid(string? status, string? id);
        Task<PaymentSessionDto?> IsSessionValid(string? id);
        Task<object?> ResetDownload(DownloadResetRequest request);
        Task<bool> IsChangeUserLevel(string? status, string? id);
        Task<LevelRequestBo?> IsSubscriptionValid(string? id, string? sessionid);
        Task<bool> IsRequestPaid(LevelRequestBo session);
        Task<bool> IsRequestPaid(PaymentSessionDto? dto);
        string Transform(LevelRequestBo session, string content);
        Task<string> Transform(bool isvalid, string? status, string? id, string html);
        string Transform(PaymentSessionDto? session, string html);
        Task<string> TransformForPermissions(bool isvalid, string? status, string? id, string html);
    }
}

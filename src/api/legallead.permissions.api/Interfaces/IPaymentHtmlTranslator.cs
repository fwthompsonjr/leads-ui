using legallead.permissions.api.Models;
using Stripe;

namespace legallead.permissions.api.Interfaces
{
    public interface IPaymentHtmlTranslator
    {
        SubscriptionService GetSubscriptionService { get; }

        Task<DownloadResponse> GetDownloadAsync(PaymentSessionDto dto);
        Task<bool> IsRequestDownloadedAndPaidAsync(PaymentSessionDto? dto);
        Task<bool> IsRequestValidAsync(string? status, string? id);
        Task<PaymentSessionDto?> IsSessionValidAsync(string? id);
        Task<object?> ResetDownloadAsync(DownloadResetRequest request);
        Task<bool> IsChangeUserLevelAsync(string? status, string? id);
        Task<LevelRequestBo?> IsSubscriptionValidAsync(string? id, string? sessionid);
        Task<bool> IsRequestPaidAsync(LevelRequestBo session);
        Task<bool> IsRequestPaidAsync(PaymentSessionDto? dto);
        string Transform(LevelRequestBo session, string content);
        string Transform(DiscountRequestBo discountRequest, string content);
        Task<string> TransformAsync(bool isvalid, string? status, string? id, string html);
        string Transform(PaymentSessionDto? session, string html);
        Task<string> TransformForPermissionsAsync(bool isvalid, string? status, string? id, string html);
        Task<LevelRequestBo?> IsDiscountValidAsync(string? id, string? sessionid);
        Task<bool> IsDiscountPaidAsync(LevelRequestBo session);
        Task<bool> IsDiscountLevelAsync(string? status, string? id);
        Task<string> TransformForDiscountsAsync(ISubscriptionInfrastructure infra, bool isvalid, string? id, string html);
        void SetupSubscriptionService(SubscriptionService? service);
    }
}

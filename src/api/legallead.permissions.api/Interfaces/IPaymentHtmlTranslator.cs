using legallead.jdbc.entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IPaymentHtmlTranslator
    {
        Task<bool> IsRequestValid(string? status, string? id);
        Task<PaymentSessionDto?> IsSessionValid(string? id);
        Task<string> Transform(bool isvalid, string? status, string? id, string html);
        string Transform(PaymentSessionDto? session, string html);
    }
}

namespace legallead.permissions.api.Interfaces
{
    public interface IPaymentHtmlTranslator
    {
        bool IsRequestValid(string? status, string? id);
    }
}

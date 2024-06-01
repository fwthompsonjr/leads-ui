using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IClientSecretService
    {
        string GetDiscountSecret(DiscountRequestBo requested, string paymentType = "Monthly");
        string GetSubscriptionSecret(LevelRequestBo requested, string paymentType = "Monthly");
    }
}

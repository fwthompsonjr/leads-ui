using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Utility
{
    public class ClientSecretService(
        ICustomerRepository customerRepo,
        IUserRepository userRepo,
        IUserSearchRepository searchRepo) : IClientSecretService
    {
        private readonly ICustomerRepository customerDb = customerRepo;
        private readonly IUserRepository userDb = userRepo;
        private readonly IUserSearchRepository? searchDb = searchRepo;

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
        public string GetDiscountSecret(DiscountRequestBo requested, string paymentType = "Monthly")
        {
            var response =
                StripeDiscountRetryService.CreatePaymentAsync(
                    requested,
                    paymentType,
                    customerDb,
                    userDb,
                    searchDb).GetAwaiter().GetResult();
            return response?.ClientSecret ?? string.Empty;
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
        public string GetSubscriptionSecret(LevelRequestBo requested, string paymentType = "Monthly")
        {
            var response =
                StripeSubscriptionRetryService.CreatePaymentAsync(
                    requested,
                    paymentType,
                    customerDb,
                    userDb,
                    searchDb).GetAwaiter().GetResult();
            return response?.ClientSecret ?? string.Empty;
        }
    }
}

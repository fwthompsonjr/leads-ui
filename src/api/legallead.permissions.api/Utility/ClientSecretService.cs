using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;
using Microsoft.VisualStudio.Shell;

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
            var jwt = ThreadHelper.JoinableTaskFactory;
            var response = jwt.Run(async delegate
            {
                var answer = await StripeDiscountRetryService.CreatePaymentAsync(
                    requested,
                    paymentType,
                    customerDb,
                    userDb,
                    searchDb);
                return answer;
            });
            return response?.ClientSecret ?? string.Empty;
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
        public string GetSubscriptionSecret(LevelRequestBo requested, string paymentType = "Monthly")
        {
            var jwt = ThreadHelper.JoinableTaskFactory;
            var response = jwt.Run(async delegate
            {
                var answer = await StripeSubscriptionRetryService.CreatePaymentAsync(
                    requested,
                    paymentType,
                    customerDb,
                    userDb,
                    searchDb);
                return answer;
            });
            return response?.ClientSecret ?? string.Empty;
        }
    }
}

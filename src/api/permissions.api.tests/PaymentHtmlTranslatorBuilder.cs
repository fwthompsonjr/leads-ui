using legallead.jdbc.interfaces;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace permissions.api.tests
{
    internal class PaymentHtmlTranslatorBuilder : IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly IPaymentHtmlTranslator _translator;
        private readonly IUserSearchRepository _repo;
        private readonly ISubscriptionInfrastructure _subscriptionDb;
        private readonly ICustomerInfrastructure _custDb;
        private readonly IUserRepository _userDb;
        private readonly Mock<IUserSearchRepository> _mockrepo;
        private readonly Mock<ISubscriptionInfrastructure> _mocksubscriptionDb;
        private readonly Mock<ICustomerInfrastructure> _mockcustDb;
        private readonly Mock<IUserRepository> _mockuserDb;
        private readonly Mock<SubscriptionService> _subscriptionService;
        private bool disposedValue;

        public PaymentHtmlTranslatorBuilder()
        {
            _provider = PaymentHtmlHelper.GetProvider();
            _translator = _provider.GetRequiredService<PaymentHtmlTranslator>();
            _repo = _provider.GetRequiredService<IUserSearchRepository>();
            _subscriptionDb = _provider.GetRequiredService<ISubscriptionInfrastructure>();
            _custDb = _provider.GetRequiredService<ICustomerInfrastructure>();
            _userDb = _provider.GetRequiredService<IUserRepository>();
            _mockrepo = _provider.GetRequiredService<Mock<IUserSearchRepository>>();
            _mocksubscriptionDb = _provider.GetRequiredService<Mock<ISubscriptionInfrastructure>>();
            _mockcustDb = _provider.GetRequiredService<Mock<ICustomerInfrastructure>>();
            _mockuserDb = _provider.GetRequiredService<Mock<IUserRepository>>();
            _subscriptionService = _provider.GetRequiredService<Mock<SubscriptionService>>();
        }

        public IServiceProvider Provider => _provider;
        public IPaymentHtmlTranslator Translator => _translator;
        public IUserSearchRepository Repo => _repo;
        public ISubscriptionInfrastructure SubscriptionDb => _subscriptionDb;
        public ICustomerInfrastructure CustDb => _custDb;
        public IUserRepository UserDb => _userDb;
        public Mock<IUserSearchRepository> MockRepo => _mockrepo;
        public Mock<ISubscriptionInfrastructure> MockSubscriptionDb => _mocksubscriptionDb;
        public Mock<ICustomerInfrastructure> MockCustDb => _mockcustDb;
        public Mock<IUserRepository> MockUserDb => _mockuserDb;
        public Mock<SubscriptionService> MockSubscriptionService => _subscriptionService;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    InvoiceExtensions.GetInfrastructure = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;
using Stripe;

namespace legallead.permissions.api.Services
{
    public class LeadInvoiceService(
        IInvoiceRepository repo,
        PaymentStripeOption payment) : ILeadInvoiceService
    {
        private readonly IInvoiceRepository db = repo;
        private readonly PaymentStripeOption stripeoption = payment;
        public Task<GetInvoiceResponse?> GetByCustomerIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<GetInvoiceResponse?> GetByInvoiceIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LeadCustomerBo>?> GetOrCreateAccountAsync(CreateInvoiceAccountModel query)
        {
            if (string.IsNullOrEmpty(query.LeadId))
                throw new ArgumentOutOfRangeException(nameof(query));

            if (string.IsNullOrEmpty(query.EmailAcct))
                throw new ArgumentOutOfRangeException(nameof(query));
            query.IsTesting = IsTestPayment;

            var found = await db.FindAccountAsync(new LeadCustomerBo { 
                Id = query.LeadId,
                Email = query.EmailAcct,
                IsTest = query.IsTesting
            });
            if (found != null && found.Count > 0 && found.Exists(x => !string.IsNullOrEmpty(x.CustomerId))) return found;
            var account = CreatePaymentAccount(query);
            if (string.IsNullOrEmpty(account)) return null;
            var payload = new LeadCustomerBo
            {
                Email = query.EmailAcct,
                IsTest = query.IsTesting,
                CustomerId = account,
                LeadUserId = query.LeadId,
            };
            var added = await db.CreateAccountAsync(payload);
            return added;
        }

        public Task<UpdateInvoiceResponse> UpdateInvoiceAsync(UpdateInvoiceRequest request)
        {
            throw new NotImplementedException();
        }

        protected static string CreatePaymentAccount(CreateInvoiceAccountModel model)
        {
            if (string.IsNullOrWhiteSpace(model.EmailAcct)) return string.Empty;
            try
            {
                var service = new Stripe.CustomerService();
                var options = GenerateCreateOption(model.EmailAcct);
                var account = service.Create(options);
                return account?.Id ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        protected static CustomerCreateOptions GenerateCreateOption(string email)
        {
            return new CustomerCreateOptions
            {
                Description = "Legal Lead Customer",
                Email = email
            };
        }
        private bool IsTestPayment
        {
            get
            {
                if (isTesting.HasValue) return isTesting.Value;
                var option = stripeoption.Key;
                if (string.IsNullOrEmpty(option)) option = "test";
                isTesting = option.Contains("test", StringComparison.OrdinalIgnoreCase);
                return isTesting.Value;
            }
        }


        private bool? isTesting;
    }
}

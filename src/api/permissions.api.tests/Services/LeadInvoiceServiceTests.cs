using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Moq;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Services
{
    public class LeadInvoiceServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var sut = new InvoiceServiceMock();
            Assert.NotNull(sut.Service);
            Assert.NotNull(sut.Repository);
            Assert.NotNull(sut.MqPaymentOptions);
        }
        private sealed class InvoiceServiceMock
        {
            private readonly LeadInvoiceService _service;
            private readonly Mock<IInvoiceRepository> _repository;
            private readonly Mock<PaymentStripeOption> _pmt;

            public InvoiceServiceMock()
            {
                _pmt = new Mock<PaymentStripeOption>();
                _repository = new Mock<IInvoiceRepository>();
                _service = new LeadServiceMock(_repository.Object, _pmt.Object);
            }

            public LeadInvoiceService Service => _service;
            public Mock<IInvoiceRepository> Repository => _repository;
            public Mock<PaymentStripeOption> MqPaymentOptions => _pmt;
        }

        private sealed class LeadServiceMock(
        IInvoiceRepository repo,
        PaymentStripeOption payment) : LeadInvoiceService(repo, payment)
        {
            protected override string CreatePaymentAccount(CreateInvoiceAccountModel model)
            {
                return faker.Random.Guid().ToString();
            }

            protected override Invoice? CreateInvoice(string customerId, InvoiceHeaderModel model, List<InvoiceDetailModel> lines)
            {
                if (model.InvoiceTotal < 0.50m)
                    return base.CreateInvoice(customerId, model, lines);
                return new()
                {
                    Id = faker.Random.AlphaNumeric(10),
                    InvoicePdf = faker.Internet.Url()
                };
            }
            private static readonly Faker faker = new Faker();
        }
    }
}
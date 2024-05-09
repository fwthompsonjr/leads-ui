using legallead.jdbc.entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using Newtonsoft.Json;

namespace permissions.api.tests
{
    public class ExcelExtensionsTests
    {
        private static readonly Faker<SearchInvoiceBo> invoiceFaker
            = new Faker<SearchInvoiceBo>()
            .RuleFor(x => x.LineId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 5))
            .RuleFor(x => x.UnitPrice, y => y.Random.Int(1, 10))
            .RuleFor(x => x.ReferenceId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent());


        private static readonly Faker<PaymentSessionJs> paymentfaker = new Faker<PaymentSessionJs>()
            .RuleFor(x => x.Description, y => y.Person.UserName)
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.SuccessUrl, y => y.Random.Guid().ToString())
            .FinishWith((a, b) =>
            {
                var invoice = invoiceFaker.Generate(a.Random.Int(1, 4));
                b.Data = invoice;
            });
        private static readonly Faker<PaymentSessionDto> sessionfaker = new Faker<PaymentSessionDto>()
            .RuleFor(x => x.JsText, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.ClientId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .FinishWith((a, b) =>
            {
                var data = paymentfaker.Generate();
                b.JsText = JsonConvert.SerializeObject(data);
            });

        private static readonly Faker<SearchFinalBo> searchfaker = new Faker<SearchFinalBo>()
            .RuleFor(x => x.Plantiff, y => y.Person.FullName)
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.DateFiled, y => y.Date.Recent().ToString("MM/dd/yyyy"))
            .RuleFor(x => x.Address1, y => y.Person.Address.Street)
            .RuleFor(x => x.Address2, y =>
            {
                var isEmpty = y.Random.Bool();
                if (isEmpty) return string.Empty;
                return y.Person.Address.Suite;
            })
            .RuleFor(x => x.Address3, y =>
            {
                var address = y.Person.Address;
                return $"{address.City}, {address.State} {address.ZipCode}";
            });


        [Fact]
        public void SessionCanGenerate()
        {
            var exception = Record.Exception(() => { _ = sessionfaker.Generate(); });
            Assert.Null(exception);
        }

        [Fact]
        public void AddressCanGenerate()
        {
            var exception = Record.Exception(() => { _ = searchfaker.Generate(); });
            Assert.Null(exception);
        }

        [Fact]
        public void ExcelCanBeCreated()
        {
            var dto = sessionfaker.Generate();
            var records = searchfaker.Generate(new Faker().Random.Int(5, 50));
            var response = ExcelExtensions.WriteExcel(dto, records);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
        }

    }
}

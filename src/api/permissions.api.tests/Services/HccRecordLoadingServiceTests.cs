using legallead.jdbc.interfaces;
using legallead.permissions.api.Services;
using Stripe;
using System.Text;

namespace permissions.api.tests.Services
{
    public class HccRecordLoadingServiceTests
    {
        [Fact]
        public void ServiceCanBeConstructed()
        {
            var service = GetService();
            Assert.NotNull(service);
        }
        [Fact]
        public async Task ServiceCanLoadAsync()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var service = GetService();
                await service.LoadAsync();
            });
            Assert.Null(error);
        }
        private static HccRecordLoadingService GetService()
        {
            var obj = faker.Generate().Data;
            var dat = Encoding.UTF8.GetBytes(obj);
            var conversion = Convert.ToBase64String(dat);
            var mock = new Mock<IHarrisLoadRepository>();
            var positive = new KeyValuePair<bool, string>(true, "unit test");
            mock.Setup(s => s.Append(It.IsAny<string>())).ReturnsAsync(positive);
            return new HccRecordLoadingService(conversion, mock.Object, 4);
        }

        private static readonly Faker<MockCsvRecordDto> recfaker =
            new Faker<MockCsvRecordDto>()
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(12))
            .RuleFor(x => x.FilingDate, y => y.Date.Recent().ToString("yyyyMMdd"))
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.StreetNumber, y => y.Random.Int(1, 9999).ToString())
            .RuleFor(x => x.StreetName, y => y.Address.StreetName())
            .RuleFor(x => x.City, y => y.Address.City())
            .RuleFor(x => x.State, y => y.Address.State())
            .RuleFor(x => x.ZipCode, y => y.Address.ZipCode())
            .RuleFor(x => x.Court, y => y.Random.Int(1, 16).ToString("000"));
        private static readonly Faker<MockCsvDto> faker =
            new Faker<MockCsvDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Data, y => y.Hacker.Phrase())
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 250000))
            .FinishWith((f, obj) =>
            {
                var names = "cas,fda,crt,def_nam,def_stnum,def_stnam,def_cty,def_st,def_zip";
                var list = new List<string>
                {
                    string.Join("\t", names.Split(','))
                };
                var dta = recfaker.Generate(10);
                dta.ForEach(d =>
                {
                    var tmp = new[]
                    {
                        d.CaseNumber,
                        d.FilingDate,
                        d.Court,
                        d.Name,
                        d.StreetNumber,
                        d.StreetName,
                        d.City,
                        d.State,
                        d.ZipCode
                    };
                    list.Add(string.Join("\t", tmp));
                });
                obj.Data = string.Join(Environment.NewLine, list);
            });
        private sealed class MockCsvDto
        {
            public string Id { get; set; } = string.Empty;
            public int? RecordCount { get; set; } = null;
            public string Data { get; set; } = string.Empty;
        }

        private sealed class MockCsvRecordDto
        {
            public string CaseNumber { get; set; } = string.Empty;
            public string FilingDate { get; set; } = string.Empty;
            public string Court { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string StreetNumber { get; set; } = string.Empty;
            public string StreetName { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string State { get; set; } = string.Empty;
            public string ZipCode { get; set; } = string.Empty;
        }
    }
}
using legallead.email.models;

namespace legallead.email.tests.models
{
    public class PermissionDiscountRequestTests
    {
        private static readonly List<string> Counties =
            [.. "Harris,Denton,Tarrant,Dallas,Travis,Houston".Split(',')];

        private static readonly Faker<PermissionDiscountChoice> dcfaker =
            new Faker<PermissionDiscountChoice>()
                .RuleFor(x => x.IsSelected, y => y.Random.Bool())
            .FinishWith((a, b) =>
            {
                var item = a.PickRandom(Counties);
                b.StateName = "TX";
                b.CountyName = item;
            });

        private static readonly Faker<PermissionDiscountRequest> faker
            = new Faker<PermissionDiscountRequest>()
            .RuleFor(x => x.Choices, y =>
            {
                var n = y.Random.Int(1, 10);
                return dcfaker.Generate(n);
            });


        [Fact]
        public void DtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DtoHasExpectedFieldDefined()
        {
            var sut = new PermissionDiscountRequest();
            var test = faker.Generate();
            Assert.NotEqual(sut.Choices, test.Choices);
        }
    }
}

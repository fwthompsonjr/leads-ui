using legallead.email.models;

namespace legallead.email.tests.models
{
    public class PermissionDiscountChoiceTests
    {
        private static readonly List<string> Counties =
            [.. "Harris,Denton,Tarrant,Dallas,Travis,Houston".Split(',')];

        private static readonly Faker<PermissionDiscountChoice> faker =
            new Faker<PermissionDiscountChoice>()
                .RuleFor(x => x.IsSelected, y => y.Random.Bool())
            .FinishWith((a, b) =>
            {
                var item = a.PickRandom(Counties);
                b.StateName = "TX";
                b.CountyName = item;
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
            var sut = new PermissionDiscountChoice();
            var test = faker.Generate();
            sut.IsSelected = test.IsSelected;
            Assert.Equal(sut.IsSelected, test.IsSelected);
            Assert.NotEqual(sut.StateName, test.StateName);
            Assert.NotEqual(sut.CountyName, test.CountyName);
        }
    }
}
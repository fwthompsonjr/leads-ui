using legallead.jdbc.entities;
using legallead.permissions.api.Services;

namespace permissions.api.tests.Services
{
    public class PricingLookupServiceTests
    {
        private static readonly List<string> CodeNames = "A,B,C,D".Split(',').ToList();
        private static readonly Faker<PricingCodeBo> faker =
            new Faker<PricingCodeBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.KeyName, y => y.PickRandom(CodeNames));

        private static List<PricingCodeBo> GetList()
        {
            var list = faker.Generate(50);
            var unique = list.Select(x => x.KeyName).Distinct().ToList();
            unique.ForEach(u =>
            {
                var source = list.Find(a => a.KeyName == u);
                list.ForEach(a =>
                {
                    if (a.KeyName == u && source != null) a.Id = source.Id;
                });
            });
            return list;
        }

        [Fact]
        public void ListContainsUniqueValues()
        {
            var list = GetList();
            PricingLookupService.Append(list);
            var lookup = PricingLookupService.PricingCodes;
            var unique = lookup.Select(x => x.KeyName).Distinct().ToList();
            Assert.Equal(unique.Count, lookup.Count);
        }
    }
}

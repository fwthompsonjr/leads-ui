using Bogus;
using legallead.jdbc.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.tests.entities
{
    public class SearchFinalBoTest
    {
        private static readonly Faker<SearchFinalBo> faker =
            new Faker<SearchFinalBo>()
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.Zip, y => y.Random.Int(0,99999).ToString("D5"))
            .RuleFor(x => x.Address1, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Address2, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Address3, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CaseNumber, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.DateFiled, y => y.Date.Recent().ToString("F"))
            .RuleFor(x => x.Court, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CaseType, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CaseStyle, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.FirstName, y => y.Person.FirstName)
            .RuleFor(x => x.LastName, y => y.Person.LastName)
            .RuleFor(x => x.Plantiff, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.County, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CourtAddress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Status, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void SearchFinalBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchFinalBo();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchFinalBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }
    }
}

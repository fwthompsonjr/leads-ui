using legallead.jdbc.entities;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Entities
{
    public class MailboxRequestTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(0, true, false)]
        [InlineData(1, true, false)]
        [InlineData(2, true, false)]
        [InlineData(3, true, false)]
        [InlineData(4, true, false)]
        [InlineData(5, true, false)]
        [InlineData(0, true, true, false)]
        [InlineData(1, true, true, false)]
        [InlineData(2, true, true, false)]
        [InlineData(3, true, true, false)]
        [InlineData(4, true, true, false)]
        [InlineData(5, true, true, false)]
        public void ItemValidationTest(int requestId, 
            bool hasUser = true,
            bool hasMessage = true,
            bool hasSearchDate = true)
        {
            var sut = new MailboxRequest();
            var faker = new Faker();
            sut.LastUpdate = hasSearchDate ? faker.Date.Recent() : null;
            sut.UserId = hasUser ? faker.Random.Guid().ToString() : string.Empty;
            sut.MessageId = hasMessage ? faker.Random.Guid().ToString() : string.Empty;
            var name = requestId switch
            {
                0 => _requests[0],
                1 => _requests[1],
                2 => _requests[2],
                3 => string.Empty,
                4 => "   ",
                _ => faker.Random.AlphaNumeric(10),
            };
            sut.RequestType = name;
            var problem = Record.Exception(() =>
            {
                _ = sut.IsValid();
            });
            Assert.Null(problem);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void ItemValidationNonGuidTest(int requestId,
            bool hasUser = true,
            bool hasMessage = true,
            bool hasSearchDate = true)
        {
            var sut = new MailboxRequest();
            var faker = new Faker();
            sut.LastUpdate = hasSearchDate ? faker.Date.Recent() : null;
            sut.UserId = hasUser ? faker.Random.AlphaNumeric(10) : string.Empty;
            sut.MessageId = hasMessage ? faker.Random.AlphaNumeric(10) : string.Empty;
            var name = requestId switch
            {
                0 => _requests[0],
                1 => _requests[1],
                2 => _requests[2],
                3 => string.Empty,
                4 => "   ",
                _ => faker.Random.AlphaNumeric(10),
            };
            sut.RequestType = name;
            var problem = Record.Exception(() =>
            {
                _ = sut.IsValid();
            });
            Assert.Null(problem);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ItemExtendedValidation(int userType)
        {
            var user = userType switch
            {
                0 => null,
                1 => new User(),
                _ => new User { Id = Guid.NewGuid().ToString() }
            };
            var mock = new Mock<ISearchInfrastructure>();
            var http = new Mock<HttpRequest>();
            mock.Setup(x => x.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            var sut = faker1.Generate();
            var problem = Record.Exception(() =>
            {
                _ = sut.IsValid(mock.Object, http.Object);
            });
            Assert.Null(problem);
            
        }

        private static readonly Faker<MailboxRequest> faker1 =
            new Faker<MailboxRequest>()
            .RuleFor(x => x.RequestType, y => y.PickRandom(_requests))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.MessageId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.LastUpdate, y => y.Date.Recent(60));

        private static readonly List<string> _requests =
            ["body", "count", "messages"];
    }
}

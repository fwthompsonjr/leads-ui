using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class HarrisLoadRepositoryTests
    {

        private static readonly Faker<HarrisCriminalUploadDto> faker
            = new Faker<HarrisCriminalUploadDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CourtDivisionIndicator, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CaseFileDate, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.InstrumentType, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.SettingResults, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CourtNumber, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CaseStatus, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantStatus, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.BondAmount, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CurrentOffenseCode, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CurrentOffenseLiteral, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CurrentOffenseLevelAndDegree, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ComplaintOffenseCode, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ComplaintOffenseLiteral, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ComplaintOffenseLevelAndDegree, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.GrandJuryOffenseCode, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.GrandJuryOffenseLiteral, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.GrandJuryOffenseLevelAndDegree, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.NextAppearanceDate, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DocketType, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.NextAppearanceReason, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantSpn, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantRace, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantSex, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantDateOfBirth, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantStreetNumber, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantStreetName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantApartmentNumber, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantCity, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantState, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantZip, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.AttorneyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.AttorneySpn, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.AttorneyConnectionCode, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.AttorneyConnectionLiteral, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ComplainantName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ComplainantAgency, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.OffenseReportNumber, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DispositionDate, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Disposition, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Sentence, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefCitizenshipStatus, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.BondException, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.GrandJuryDate, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.GrandJuryCourt, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.GrandJuryAction, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DefendantPlaceOfBirth, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }


        [Fact]
        public async Task RepoCanAppendHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.Append("test-payload");
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanAppendExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.Append("test-payload");
            Assert.False(response.Key);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public async Task RepoCanFindHappyPath(int recordcount)
        {
            var completion = faker.Generate(recordcount);
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.QueryAsync<HarrisCriminalUploadDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.Find(DateTime.Now);
            Assert.NotNull(response);
        }
        private sealed class RepoContainer
        {
            private readonly IHarrisLoadRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new HarrisLoadRepository(dataContext);
            }

            public IHarrisLoadRepository Repository => repo;
            public Mock<IDapperCommand> CommandMock => command;
        }
    }
}
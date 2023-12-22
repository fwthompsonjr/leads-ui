using Bogus;
using Dapper;
using legallead.logging.entities;
using legallead.logging.implementations;
using legallead.logging.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.logging.tests.implementations
{
    public class LogContentRepositoryTests
    {
        private const int seed = 100;

        private readonly Faker<InsertIndexDto> inderFaker =
            new Faker<InsertIndexDto>()
            .RuleFor(x => x.Id, y => y.IndexGlobal);

        private readonly Faker<LogInsertModel> modelFaker =
            new Faker<LogInsertModel>()
            .RuleFor(x => x.RequestId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StatusId, y => y.Random.Int(10, 2000))
            .RuleFor(x => x.LineNumber, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.NameSpace, y => y.Company.CompanyName())
            .RuleFor(x => x.ClassName, y => y.Company.CompanyName())
            .RuleFor(x => x.MethodName, y => y.Company.CompanyName())
            .RuleFor(x => x.Message, y => y.Company.CompanyName())
            .RuleFor(x => x.Detail, y => y.Lorem.Sentence(2000));

        private readonly Faker<LogContentDetailDto> detailFaker =
            new Faker<LogContentDetailDto>()
            .RuleFor(x => x.Id, y => y.IndexFaker + seed)
            .RuleFor(x => x.LogContentId, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.LineId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.Line, y => y.Company.CompanyName());

        [Fact]
        public async Task LogCanInsertChild()
        {
            var query = detailFaker.Generate();

            var context = new Mock<ILoggingDbContext>();
            var db = new Mock<ILoggingDbCommand>();
            var conn = new Mock<IDbConnection>();
            var sut = new TestInsertRepo(context.Object, db.Object);

            context.SetupGet(m => m.GetCommand).Returns(db.Object);
            context.Setup(m => m.CreateConnection()).Returns(conn.Object);

            var exception = await Record.ExceptionAsync(async () => { await sut.InsertChild(query); });
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task LogCanInsertHappyPath()
        {
            var query = modelFaker.Generate();
            var index = inderFaker.Generate();

            var context = new Mock<ILoggingDbContext>();
            var db = new Mock<ILoggingDbCommand>();
            var conn = new Mock<IDbConnection>();
            var sut = new TestInsertRepo(context.Object, db.Object);

            context.SetupGet(m => m.GetCommand).Returns(db.Object);
            context.Setup(m => m.CreateConnection()).Returns(conn.Object);

            db.Setup(m => m.QuerySingleOrDefaultAsync<InsertIndexDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(index);

            var exception = await Record.ExceptionAsync(async () => { await sut.Insert(query); });
            Assert.NotNull(exception);
        }

        private sealed class TestInsertRepo : LogContentRepository
        {
            private readonly ILoggingDbCommand mockCommand;

            public TestInsertRepo(ILoggingDbContext context, ILoggingDbCommand cmmd) : base(context)
            {
                mockCommand = cmmd;
            }

            public override ILoggingDbCommand GetCommand => mockCommand;

            public override async Task InsertChild(LogContentDetailDto dto)
            {
                try
                {
                    await base.InsertChild(dto);
                }
                catch
                {
                    // no exceptions to be thrown from this test method
                }
            }
        }
    }
}
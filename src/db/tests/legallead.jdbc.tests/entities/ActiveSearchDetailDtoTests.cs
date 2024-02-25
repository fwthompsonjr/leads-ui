using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class ActiveSearchDetailDtoTests
    {

        private static readonly Faker<ActiveSearchDetailDto> faker =
            new Faker<ActiveSearchDetailDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateName, y => y.Hacker.Phrase())
            .RuleFor(x => x.StartDate, y => y.Hacker.Phrase())
            .RuleFor(x => x.EndDate, y => y.Hacker.Phrase())
            .RuleFor(x => x.SearchProgress, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void ActiveSearchDetailDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ActiveSearchDetailDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ActiveSearchDetailDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ActiveSearchDetailDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void ActiveSearchDetailDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void ActiveSearchDetailDtoCanSetCountyName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CountyName = src.CountyName;
            Assert.Equal(src.CountyName, dest.CountyName);
        }
        [Fact]
        public void ActiveSearchDetailDtoCanSetStateName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StateName = src.StateName;
            Assert.Equal(src.StateName, dest.StateName);
        }

        [Fact]
        public void ActiveSearchDetailDtoCanSetStartDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StartDate = src.StartDate;
            Assert.Equal(src.StartDate, dest.StartDate);
        }

        [Fact]
        public void ActiveSearchDetailDtoCanSetEndDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.EndDate = src.EndDate;
            Assert.Equal(src.EndDate, dest.EndDate);
        }

        [Fact]
        public void ActiveSearchDetailDtoCanSetSearchProgress()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SearchProgress = src.SearchProgress;
            Assert.Equal(src.SearchProgress, dest.SearchProgress);
        }



        [Fact]
        public void ActiveSearchDetailDtoIsBaseDto()
        {
            var sut = new ActiveSearchDetailDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("CountyName")]
        [InlineData("StateName")]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("SearchProgress")]
        [InlineData("CreateDate")]
        public void ActiveSearchDetailDtoHasExpectedFieldDefined(string name)
        {
            var sut = new ActiveSearchDetailDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("CountyName")]
        [InlineData("StateName")]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("SearchProgress")]
        [InlineData("CreateDate")]
        public void ActiveSearchDetailDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new ActiveSearchDetailDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}

﻿using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class BackgroundComponentStatusDtoTests
    {

        private static readonly Faker<BackgroundComponentStatusDto> faker =
            new Faker<BackgroundComponentStatusDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ComponentId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(0, 30000))
            .RuleFor(x => x.StatusId, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.StatusName, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void BackgroundComponentStatusDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new BackgroundComponentStatusDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void BackgroundComponentStatusDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void BackgroundComponentStatusDtoIsBaseDto()
        {
            var sut = new BackgroundComponentStatusDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void BackgroundComponentStatusDtoHasTableNameDefined()
        {
            var expected = "BGCOMPONENTSTATUS";
            var sut = new BackgroundComponentStatusDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("ComponentId")]
        [InlineData("LineNbr")]
        [InlineData("StatusName")]
        [InlineData("StatusId")]
        [InlineData("CreateDate")]
        public void BackgroundComponentStatusDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new BackgroundComponentStatusDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}
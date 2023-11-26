﻿using Bogus;
using legallead.records.search.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Globalization;

namespace legallead.records.search.Tests.Data
{
    [TestClass]
    public class HarrisCaseDateDtoTests
    {
        private Faker<HarrisCaseDateDto>? DtoFaker;

        [TestInitialize]
        public void Setup()
        {
            if (DtoFaker == null)
            {
                var minDate = DateTime.Now.Date.AddYears(-2);
                var maxDate = minDate.AddYears(2);
                DtoFaker = new Faker<HarrisCaseDateDto>()
                    .RuleFor(x => x.Interval, y =>
                    {
                        var ii = y.Random.Int(-5, 5);
                        while (ii == 0) { ii = y.Random.Int(-5, 5); }
                        return new TimeSpan(days: ii, hours: 0, minutes: 0, seconds: 0);
                    })
                    .RuleFor(x => x.StartDate, y => y.Date.Between(minDate, maxDate));
            }
        }

        [TestMethod]
        public void CanConstruct()
        {
            var obj = new HarrisCaseSearchDto();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanInit()
        {
            if (DtoFaker == null) return;
            var obj = DtoFaker.Generate();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanSet_Interval()
        {
            if (DtoFaker == null) return;
            var interval = new TimeSpan(days: -10, hours: 0, minutes: 30, seconds: 0);
            var obj = DtoFaker.Generate();

            obj.Interval = interval;
            obj.Interval.ShouldBe(interval);
        }

        [TestMethod]
        public void CanSet_StartDate()
        {
            if (DtoFaker == null) return;
            var list = DtoFaker.Generate(5);
            var obj = list[4];
            var expectedDate = list[0].StartDate;
            obj.StartDate = expectedDate;
            obj.StartDate.ShouldBe(expectedDate);
        }

        [TestMethod]
        public void CanGet_EndDate()
        {
            if (DtoFaker == null) return;
            var obj = DtoFaker.Generate();
            var expectedDate = obj.StartDate.Add(obj.Interval);
            obj.EndDate.ShouldBe(expectedDate);
        }

        [TestMethod]
        public void CanGet_StartingDate()
        {
            if (DtoFaker == null) return;
            var format = HarrisCaseDateDto.DateFormat;
            var obj = DtoFaker.Generate();
            var expectedDate = obj.StartDate.ToString(format);
            obj.StartingDate.ShouldBe(expectedDate);
        }

        [TestMethod]
        public void CanGet_EndingDate()
        {
            if (DtoFaker == null) return;
            var format = HarrisCaseDateDto.DateFormat;
            var obj = DtoFaker.Generate();
            var expectedDate = obj.StartDate.Add(obj.Interval).ToString(format, CultureInfo.CurrentCulture);
            obj.EndingDate.ShouldBe(expectedDate);
        }

        [TestMethod]
        public void CanEnumerate_StartToEnd()
        {
            if (DtoFaker == null) return;
            var totalDays = 7 * 4 * 6;
            var interval = new TimeSpan(-7, 0, 0, 0);
            var obj = DtoFaker.Generate();
            var list = HarrisCaseDateDto.BuildList(obj.StartDate, interval, totalDays);
            list.Any().ShouldBeTrue();
            list.Count.ShouldBeGreaterThan(1);
            list[0].StartDate.ShouldBe(obj.StartDate);
            list[^1].StartDate.ShouldBeLessThan(obj.StartDate);
            Math.Abs(list[0].StartDate.Subtract(list[^1].EndDate).TotalDays)
                .ShouldBeGreaterThanOrEqualTo(totalDays);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CanEnumerate_NoDay_Throws_Exception()
        {
            if (DtoFaker == null) return;
            var obj = DtoFaker.Generate();
            var interval = new TimeSpan(10, 0, 0);
            _ = HarrisCaseDateDto.BuildList(obj.StartDate, interval, 49);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CanEnumerate_NoRange_Throws_Exception()
        {
            if (DtoFaker == null) return;
            var obj = DtoFaker.Generate();
            var interval = new TimeSpan(5, 12, 0, 0);
            _ = HarrisCaseDateDto.BuildList(obj.StartDate, interval, 0);
        }
    }
}
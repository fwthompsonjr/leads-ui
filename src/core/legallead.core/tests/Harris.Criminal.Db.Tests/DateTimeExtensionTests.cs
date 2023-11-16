﻿using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Globalization;

namespace Harris.Criminal.Db.Tests
{
    [TestClass]
    public class DateTimeExtensionTests
    {
        private class SampleData
        {
            private string? _parsed;
            public string Data { get; set; } = string.Empty;
            public string Parsed => _parsed ??= Parse(Data);

            private static string Parse(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    return string.Empty;
                }

                if (data.Length != 8)
                {
                    return string.Empty;
                }

                var yy = data[..4];
                var mm = data.Substring(4, 2);
                var dd = data.Substring(6, 2);
                return $"{mm}/{dd}/{yy}";
            }
        }

        private Faker<SampleData>? Faker;

        [TestInitialize]
        public void Setup()
        {
            Faker ??= new Faker<SampleData>()
                    .RuleFor(f => f.Data, r => r.Date.Recent(-15).ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void CanParseToDate()
        {
            var items = Faker?.Generate(20);
            Assert.IsNotNull(items);
            if (items == null) return;
            items.ForEach(f =>
            {
                f.Parsed.ShouldNotBeNullOrEmpty();
                var expected = DateTime.Parse(f.Parsed, CultureInfo.InvariantCulture);
                var actual = f.Data.ToExactDate("yyyyMMdd", DateTime.MinValue);
                actual.ShouldNotBe(DateTime.MinValue);
                actual.ShouldBe(expected);
            });
        }

        [TestMethod]
        public void CanParse_EmptyString()
        {
            var input = string.Empty;
            var expected = new DateTime(2015, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var actual = input.ToExactDate("yyyyMMdd", expected);
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void CanParse_EmptyFormat()
        {
            var input = "20201225";
            var expected = new DateTime(2015, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var actual = input.ToExactDate(string.Empty, expected);
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void CanParseString_EmptyFormat()
        {
            const string fmt = "yyyyMMdd";
            var input = "20201225";
            var expected = new DateTime(2015, 1, 15, 0, 0, 0, DateTimeKind.Utc).ToString(fmt, CultureInfo.InvariantCulture);
            var actual = input.ToExactDateString(string.Empty, expected);
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void CanParseString_EmptyInput()
        {
            const string fmt = "yyyyMMdd";
            var input = string.Empty;
            var expected = new DateTime(2015, 1, 15, 0, 0, 0, DateTimeKind.Utc).ToString(fmt, CultureInfo.InvariantCulture);
            var actual = input.ToExactDateString(fmt, expected);
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void CanParseString_ValidDate()
        {
            const string fmt = "yyyyMMdd";
            var input = "20201225";
            var expected = new DateTime(2020, 12, 25, 0, 0, 0, DateTimeKind.Utc).ToString(fmt, CultureInfo.InvariantCulture);
            var actual = input.ToExactDateString(fmt, expected);
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void CanDerive_NextMonday()
        {
            var test = DateTimeExtensions.NextMonday();
            test.DayOfWeek.ShouldBe(DayOfWeek.Monday);
            test.ShouldBeGreaterThan(DateTime.Now);
            Math.Abs(test.Subtract(DateTime.Now).TotalDays).ShouldBeLessThan(8);
        }
    }
}
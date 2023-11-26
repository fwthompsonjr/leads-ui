using Bogus;
using legallead.records.search.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Globalization;

namespace legallead.records.search.Tests.Data
{
    [TestClass]
    public class CaseStyleDbParserTests
    {
        private class SytleData
        {
            public string Header { get; set; } = string.Empty;
            public string Separator { get; set; } = string.Empty;
            public string Person { get; set; } = string.Empty;
            public string Spn { get; set; } = string.Empty;
            public string Dob { get; set; } = string.Empty;

            public string CaseStyle => $"{Header} {Separator} {Person}";

            public override string? ToString()
            {
                var resp = $"{Header} {Separator} {Person} (SPN: {Spn}) (DOB: {Dob})";
                if (string.IsNullOrEmpty(resp))
                {
                    return base.ToString();
                }
                return resp;
            }
        }

        private Faker<SytleData>? FakeData;
        private CaseStyleDbParser? Parser;

        [TestInitialize]
        public void Setup()
        {
            if (FakeData == null)
            {
                var dobStart = DateTime.Now.AddYears(-80);
                var dobEnd = DateTime.Now.AddYears(-15);
                FakeData = new Faker<SytleData>()
                    .RuleFor(f => f.Header, r => $"The State of {r.Address.State()}")
                    .RuleFor(f => f.Separator, r => "vs.")
                    .RuleFor(f => f.Person, r => string.Concat(r.Person.LastName, ", ", r.Person.FirstName))
                    .RuleFor(f => f.Spn, r => r.Random.Int(100000, 9999999).ToString("d8", CultureInfo.CurrentCulture))
                    .RuleFor(f => f.Dob, r => r.Date.Between(dobStart, dobEnd).ToString("MM/dd/yyyy", CultureInfo.CurrentCulture));
            }
            if (FakeData == null) return;
            var data = FakeData.Generate();
            Parser = new CaseStyleDbParser { Data = data.ToString() ?? string.Empty };
        }

        [TestMethod]
        public void CanParse_WithVs()
        {
            if (Parser == null) return;
            Parser.CanParse().ShouldBeTrue();
        }

        [TestMethod]
        public void CanParse_WithoutData_IsFalse()
        {
            if (Parser == null) return;
            Parser.Data = string.Empty;
            Parser.CanParse().ShouldBeFalse();
        }

        [TestMethod]
        public void CanParse_WithoutVs_IsFalse()
        {
            if (FakeData == null || Parser == null) return;
            var dto = FakeData.Generate();
            dto.Separator = "versus";
            Parser.Data = dto.ToString() ?? string.Empty;
            Parser.CanParse().ShouldBeFalse();
        }

        [TestMethod]
        public void Parser_CanExtract_Defendant()
        {
            if (FakeData == null || Parser == null) return;
            var dto = FakeData.Generate(10);
            foreach (var item in dto)
            {
                Parser.Data = item.ToString() ?? string.Empty;
                var expected = item.Person;
                var response = Parser.Parse();
                response.Defendant.ShouldBe(expected);
            }
        }

        [TestMethod]
        public void Parser_CanExtract_Defendant_Once()
        {
            if (FakeData == null || Parser == null) return;
            var item = FakeData.Generate();
            Parser.Data = item.ToString() ?? string.Empty;
            var expected = item.Person;
            var response = Parser.Parse();
            response.Defendant.ShouldBe(expected);
        }

        [TestMethod]
        public void Parser_CanExtract_CaseStyle()
        {
            if (FakeData == null || Parser == null) return;
            var dto = FakeData.Generate(10);
            foreach (var item in dto)
            {
                Parser.Data = item.ToString() ?? string.Empty;
                var expected = item.CaseStyle;
                var response = Parser.Parse();
                response.CaseData.ShouldBe(expected);
            }
        }

        [TestMethod]
        public void Parser_CanExtract_CaseStyle_Once()
        {
            if (FakeData == null || Parser == null) return;
            var item = FakeData.Generate();
            Parser.Data = item.ToString() ?? string.Empty;
            var expected = item.CaseStyle;
            var response = Parser.Parse();
            response.CaseData.ShouldBe(expected);
        }

        [TestMethod]
        public void Parser_CanExtract_Plantiff()
        {
            if (FakeData == null || Parser == null) return;
            var dto = FakeData.Generate(10);
            foreach (var item in dto)
            {
                Parser.Data = item.ToString() ?? string.Empty;
                var expected = item.Header;
                var response = Parser.Parse();
                response.Plantiff.ShouldBe(expected);
            }
        }

        [TestMethod]
        public void Parser_CanExtract_Plantiff_Once()
        {
            if (FakeData == null || Parser == null) return;
            var item = FakeData.Generate();
            Parser.Data = item.ToString()!;
            var expected = item.Header;
            var response = Parser.Parse();
            response.Plantiff.ShouldBe(expected);
        }
    }
}
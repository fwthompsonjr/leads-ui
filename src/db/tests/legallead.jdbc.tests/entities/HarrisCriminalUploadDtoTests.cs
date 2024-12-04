using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class HarrisCriminalUploadDtoTests
    {
        [Fact]
        public void ModelCanCreate()
        {
            var error = Record.Exception(() =>
            {
                _ = dfaker.Generate();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("CourtDivisionIndicator")]
        [InlineData("CaseNumber")]
        [InlineData("CaseFileDate")]
        [InlineData("InstrumentType")]
        [InlineData("SettingResults")]
        [InlineData("CourtNumber")]
        [InlineData("CaseStatus")]
        [InlineData("DefendantStatus")]
        [InlineData("BondAmount")]
        [InlineData("CurrentOffenseCode")]
        [InlineData("CurrentOffenseLiteral")]
        [InlineData("CurrentOffenseLevelAndDegree")]
        [InlineData("ComplaintOffenseCode")]
        [InlineData("ComplaintOffenseLiteral")]
        [InlineData("ComplaintOffenseLevelAndDegree")]
        [InlineData("GrandJuryOffenseCode")]
        [InlineData("GrandJuryOffenseLiteral")]
        [InlineData("GrandJuryOffenseLevelAndDegree")]
        [InlineData("NextAppearanceDate")]
        [InlineData("DocketType")]
        [InlineData("NextAppearanceReason")]
        [InlineData("DefendantName")]
        [InlineData("DefendantSpn")]
        [InlineData("DefendantRace")]
        [InlineData("DefendantSex")]
        [InlineData("DefendantDateOfBirth")]
        [InlineData("DefendantStreetNumber")]
        [InlineData("DefendantStreetName")]
        [InlineData("DefendantApartmentNumber")]
        [InlineData("DefendantCity")]
        [InlineData("DefendantState")]
        [InlineData("DefendantZip")]
        [InlineData("AttorneyName")]
        [InlineData("AttorneySpn")]
        [InlineData("AttorneyConnectionCode")]
        [InlineData("AttorneyConnectionLiteral")]
        [InlineData("ComplainantName")]
        [InlineData("ComplainantAgency")]
        [InlineData("OffenseReportNumber")]
        [InlineData("DispositionDate")]
        [InlineData("Disposition")]
        [InlineData("Sentence")]
        [InlineData("DefCitizenshipStatus")]
        [InlineData("BondException")]
        [InlineData("GrandJuryDate")]
        [InlineData("GrandJuryCourt")]
        [InlineData("GrandJuryAction")]
        [InlineData("DefendantPlaceOfBirth")]
        [InlineData("CreateDate")]
        public void ModelHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = dfaker.Generate();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }


        [Theory]
        [InlineData("Id")]
        [InlineData("CourtDivisionIndicator")]
        [InlineData("CaseNumber")]
        [InlineData("CaseFileDate")]
        [InlineData("InstrumentType")]
        [InlineData("SettingResults")]
        [InlineData("CourtNumber")]
        [InlineData("CaseStatus")]
        [InlineData("DefendantStatus")]
        [InlineData("BondAmount")]
        [InlineData("CurrentOffenseCode")]
        [InlineData("CurrentOffenseLiteral")]
        [InlineData("CurrentOffenseLevelAndDegree")]
        [InlineData("ComplaintOffenseCode")]
        [InlineData("ComplaintOffenseLiteral")]
        [InlineData("ComplaintOffenseLevelAndDegree")]
        [InlineData("GrandJuryOffenseCode")]
        [InlineData("GrandJuryOffenseLiteral")]
        [InlineData("GrandJuryOffenseLevelAndDegree")]
        [InlineData("NextAppearanceDate")]
        [InlineData("DocketType")]
        [InlineData("NextAppearanceReason")]
        [InlineData("DefendantName")]
        [InlineData("DefendantSpn")]
        [InlineData("DefendantRace")]
        [InlineData("DefendantSex")]
        [InlineData("DefendantDateOfBirth")]
        [InlineData("DefendantStreetNumber")]
        [InlineData("DefendantStreetName")]
        [InlineData("DefendantApartmentNumber")]
        [InlineData("DefendantCity")]
        [InlineData("DefendantState")]
        [InlineData("DefendantZip")]
        [InlineData("AttorneyName")]
        [InlineData("AttorneySpn")]
        [InlineData("AttorneyConnectionCode")]
        [InlineData("AttorneyConnectionLiteral")]
        [InlineData("ComplainantName")]
        [InlineData("ComplainantAgency")]
        [InlineData("OffenseReportNumber")]
        [InlineData("DispositionDate")]
        [InlineData("Disposition")]
        [InlineData("Sentence")]
        [InlineData("DefCitizenshipStatus")]
        [InlineData("BondException")]
        [InlineData("GrandJuryDate")]
        [InlineData("GrandJuryCourt")]
        [InlineData("GrandJuryAction")]
        [InlineData("DefendantPlaceOfBirth")]
        [InlineData("CreateDate")]
        public void ModelCanReadWriteByIndex(string fieldName)
        {
            var demo = dfaker.Generate();
            var sut = dfaker.Generate();
            demo["id"] = null;
            sut[fieldName] = demo[fieldName];
            var actual = sut[fieldName];
            Assert.Equal(demo[fieldName], actual);
        }

        private static readonly Faker<HarrisCriminalUploadDto> dfaker
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
    }
}
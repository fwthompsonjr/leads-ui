using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class HarrisCriminalUploadBoTests
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
        [Fact]
        public void ModelHasExpectedFields()
        {
            var error = Record.Exception(() =>
            {
                var sut = dfaker.Generate();
                Assert.False(string.IsNullOrEmpty(sut.Id));
                Assert.False(string.IsNullOrEmpty(sut.CourtDivisionIndicator));
                Assert.False(string.IsNullOrEmpty(sut.CaseNumber));
                Assert.False(string.IsNullOrEmpty(sut.CaseFileDate));
                Assert.False(string.IsNullOrEmpty(sut.InstrumentType));
                Assert.False(string.IsNullOrEmpty(sut.SettingResults));
                Assert.False(string.IsNullOrEmpty(sut.CourtNumber));
                Assert.False(string.IsNullOrEmpty(sut.CaseStatus));
                Assert.False(string.IsNullOrEmpty(sut.DefendantStatus));
                Assert.False(string.IsNullOrEmpty(sut.BondAmount));
                Assert.False(string.IsNullOrEmpty(sut.CurrentOffenseCode));
                Assert.False(string.IsNullOrEmpty(sut.CurrentOffenseLiteral));
                Assert.False(string.IsNullOrEmpty(sut.CurrentOffenseLevelAndDegree));
                Assert.False(string.IsNullOrEmpty(sut.ComplaintOffenseCode));
                Assert.False(string.IsNullOrEmpty(sut.ComplaintOffenseLiteral));
                Assert.False(string.IsNullOrEmpty(sut.ComplaintOffenseLevelAndDegree));
                Assert.False(string.IsNullOrEmpty(sut.GrandJuryOffenseCode));
                Assert.False(string.IsNullOrEmpty(sut.GrandJuryOffenseLiteral));
                Assert.False(string.IsNullOrEmpty(sut.GrandJuryOffenseLevelAndDegree));
                Assert.False(string.IsNullOrEmpty(sut.NextAppearanceDate));
                Assert.False(string.IsNullOrEmpty(sut.DocketType));
                Assert.False(string.IsNullOrEmpty(sut.NextAppearanceReason));
                Assert.False(string.IsNullOrEmpty(sut.DefendantName));
                Assert.False(string.IsNullOrEmpty(sut.DefendantSpn));
                Assert.False(string.IsNullOrEmpty(sut.DefendantRace));
                Assert.False(string.IsNullOrEmpty(sut.DefendantSex));
                Assert.False(string.IsNullOrEmpty(sut.DefendantDateOfBirth));
                Assert.False(string.IsNullOrEmpty(sut.DefendantStreetNumber));
                Assert.False(string.IsNullOrEmpty(sut.DefendantStreetName));
                Assert.False(string.IsNullOrEmpty(sut.DefendantApartmentNumber));
                Assert.False(string.IsNullOrEmpty(sut.DefendantCity));
                Assert.False(string.IsNullOrEmpty(sut.DefendantState));
                Assert.False(string.IsNullOrEmpty(sut.DefendantZip));
                Assert.False(string.IsNullOrEmpty(sut.AttorneyName));
                Assert.False(string.IsNullOrEmpty(sut.AttorneySpn));
                Assert.False(string.IsNullOrEmpty(sut.AttorneyConnectionCode));
                Assert.False(string.IsNullOrEmpty(sut.AttorneyConnectionLiteral));
                Assert.False(string.IsNullOrEmpty(sut.ComplainantName));
                Assert.False(string.IsNullOrEmpty(sut.ComplainantAgency));
                Assert.False(string.IsNullOrEmpty(sut.OffenseReportNumber));
                Assert.False(string.IsNullOrEmpty(sut.DispositionDate));
                Assert.False(string.IsNullOrEmpty(sut.Disposition));
                Assert.False(string.IsNullOrEmpty(sut.Sentence));
                Assert.False(string.IsNullOrEmpty(sut.DefCitizenshipStatus));
                Assert.False(string.IsNullOrEmpty(sut.BondException));
                Assert.False(string.IsNullOrEmpty(sut.GrandJuryDate));
                Assert.False(string.IsNullOrEmpty(sut.GrandJuryCourt));
                Assert.False(string.IsNullOrEmpty(sut.GrandJuryAction));
                Assert.False(string.IsNullOrEmpty(sut.DefendantPlaceOfBirth));
                Assert.True(sut.CreateDate.HasValue);
            });
            Assert.Null(error);
        }

        private static readonly Faker<HarrisCriminalUploadBo> dfaker
            = new Faker<HarrisCriminalUploadBo>()
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
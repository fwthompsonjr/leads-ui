using Bogus;
using legallead.jdbc.models;
using System.ComponentModel.DataAnnotations;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace legallead.jdbc.tests.entities
{
    public class DbCountyFileModelTests
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
                Assert.NotEqual("", sut.Id);
                Assert.NotEqual("", sut.FileType);
                Assert.NotEqual("", sut.FileStatus);
                Assert.NotEqual("", sut.FileContent);
            });
            Assert.Null(error);
        }
        [Fact]
        public void DbCountyFileModel_ValidModel_ShouldPassValidation()
        {
            var model = new DbCountyFileModel
            {
                Id = "123",
                FileType = "CSV",
                FileStatus = "ENCODED",
                FileContent = "Sample content"
            };

            var validationResults = ValidateModel(model);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void DbCountyFileModel_InvalidFileType_ShouldFailValidation()
        {
            var model = new DbCountyFileModel
            {
                Id = "123",
                FileType = "INVALID",
                FileStatus = "ENCODED",
                FileContent = "Sample content"
            };

            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(DbCountyFileModel.FileType)));
        }

        [Fact]
        public void DbCountyFileModel_InvalidFileStatus_ShouldFailValidation()
        {
            var model = new DbCountyFileModel
            {
                Id = "123",
                FileType = "CSV",
                FileStatus = "INVALID",
                FileContent = "Sample content"
            };

            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(DbCountyFileModel.FileStatus)));
        }

        private static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        private static readonly Faker<DbCountyFileModel> dfaker
            = new Faker<DbCountyFileModel>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.FileType, y => y.Lorem.Word())
            .RuleFor(x => x.FileStatus, y => y.Lorem.Word())
            .RuleFor(x => x.FileContent, y => y.Hacker.Phrase());
    }
}
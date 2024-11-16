using legallead.permissions.api;
using legallead.permissions.api.Model;
using System.Text;

namespace permissions.api.tests.Models
{
    public class RegisterAccountModelTests
    {
        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new RegisterAccountModel();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = LeadUserBoGenerator.GetAccount();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGetUserName()
        {
            var test = LeadUserBoGenerator.GetAccount();
            Assert.False(string.IsNullOrEmpty(test.UserName));
        }

        [Fact]
        public void ModelCanSetUserName()
        {
            var test = LeadUserBoGenerator.GetAccount();
            var source = LeadUserBoGenerator.GetAccount();
            test.UserName = source.UserName;
            Assert.Equal(source.UserName, test.UserName);
        }

        [Fact]
        public void ModelCanGetPassword()
        {
            var test = LeadUserBoGenerator.GetAccount();
            Assert.False(string.IsNullOrEmpty(test.Password));
        }

        [Fact]
        public void ModelCanSetPassword()
        {
            var test = LeadUserBoGenerator.GetAccount();
            var source = LeadUserBoGenerator.GetAccount();
            test.Password = source.Password;
            Assert.Equal(source.Password, test.Password);
        }

        [Fact]
        public void ModelCanGetEmail()
        {
            var test = LeadUserBoGenerator.GetAccount();
            Assert.False(string.IsNullOrEmpty(test.Email));
        }

        [Fact]
        public void ModelCanSetEmail()
        {
            var test = LeadUserBoGenerator.GetAccount();
            var source = LeadUserBoGenerator.GetAccount();
            test.Email = source.Email;
            Assert.Equal(source.Email, test.Email);
        }

        [Fact]
        public void ModelIsInValidWithEmptyUserName()
        {
            var test = LeadUserBoGenerator.GetAccount();
            test.UserName = string.Empty;
            var results = test.Validate(out bool isValid);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void ModelIsInValidWithEmptyPassword()
        {
            var test = LeadUserBoGenerator.GetAccount();
            test.Password = string.Empty;
            var results = test.Validate(out bool isValid);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void ModelIsInValidWithEmptyEmail()
        {
            var test = LeadUserBoGenerator.GetAccount();
            test.Email = string.Empty;
            var results = test.Validate(out bool isValid);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void ModelDefaultIsValid()
        {
            var test = LeadUserBoGenerator.GetAccount();
            var results = test.Validate(out bool isValid);
            Assert.True(isValid);
            Assert.Empty(results);
        }

    }
}
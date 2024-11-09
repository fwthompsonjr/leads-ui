using legallead.permissions.api.Services;

namespace permissions.api.tests.Services
{
    public class AppAuthenicationServiceTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("", "empty")]
        [InlineData("missing", "empty")]
        public void ServiceCanAuthenicate(string username, string password)
        {
            var service = new AppAuthenicationService();
            var actual = service.Authenicate(username, password);
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("", 1, false)]
        [InlineData("empty", 1, false)]
        [InlineData("lead.administrator", 1, false)]
        [InlineData("lead.administrator", 0, true)]
        [InlineData("Kerri@kdphillipslaw.com", 1, true)]
        [InlineData("kerri@kdphillipslaw.com", 1, true)]
        public void ServiceCanFindUser(string username, int index, bool expected)
        {
            var service = new AppAuthenicationService();
            var user = service.FindUser(username, index);
            var actual = user != null;
            Assert.Equal(expected, actual);
        }
    }
}

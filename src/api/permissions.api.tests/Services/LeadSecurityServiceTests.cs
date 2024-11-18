using legallead.jdbc.entities;
using legallead.permissions.api.Services;

namespace permissions.api.tests.Services
{
    public class LeadSecurityServiceTests
    {
        [Theory]
        [InlineData(0, 5)]
        [InlineData(1, 5)]
        [InlineData(2, 5)]
        [InlineData(0, -1)]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        public void SerivceCanGetModel(int counties, int indexes)
        {
            var error = Record.Exception(() =>
            {
                var bo = GetBo(counties, indexes);
                var model = leadSvcs.GetModel(bo);
                Assert.False(string.IsNullOrEmpty(model.Id));
            });
            Assert.Null(error);
        }

        [Fact]
        public void SerivceCanGetUserData()
        {
            // thread check
            var error = Record.Exception(() =>
            {
                var bo = GetBo();
                var userData = leadSvcs.GetUserData(leadSvcs.GetModel(bo));
                Assert.False(string.IsNullOrEmpty(userData));
            });
            Assert.Null(error);
        }

        private static LeadUserBo GetBo(int countiesCount = 2, int indexCount = 0)
        {
            return LeadUserBoGenerator.GetBo(countiesCount, indexCount);
        }


        private static readonly LeadSecurityService leadSvcs = new();
    }
}

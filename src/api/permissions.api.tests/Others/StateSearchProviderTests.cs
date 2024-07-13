using legallead.permissions.api.Utility;

namespace permissions.api.tests
{
    public class StateSearchProviderTests
    {
        [Fact]
        public void ProviderCanGetStates()
        {
            const int activeCounties = 4;
            var sut = new StateSearchProvider();
            var list = sut.GetStates();
            Assert.NotNull(list);
            Assert.Single(list);
            Assert.NotNull(list[0].Counties);
            Assert.Equal(activeCounties, list[0].Counties?.Count);
        }
    }
}
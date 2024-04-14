using legallead.permissions.api.Models;

namespace permissions.api.tests.Models
{
    public class SubscriptionCreatedModelTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ModelWillInitializeField(int id)
        {
            var model = new SubscriptionCreatedModel();
            if (id == 0) Assert.False(string.IsNullOrEmpty(model.Id));
            if (id == 1) Assert.False(string.IsNullOrEmpty(model.Url));
        }
    }
}

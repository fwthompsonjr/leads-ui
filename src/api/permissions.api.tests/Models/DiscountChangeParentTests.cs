using legallead.permissions.api.Models;

namespace permissions.api.tests.Models
{
    public class DiscountChangeParentTests
    {
        [Fact]
        public void ModelCanCreate()
        {
            var model = new DiscountChangeParent();
            Assert.NotNull(model);
            Assert.Empty(model.Choices);
        }
    }
}

using legallead.permissions.api.Utility;

namespace permissions.api.tests.Others
{
    public class RoleDescriptionsTests
    {
        [Theory]
        [InlineData("", "Guest")]
        [InlineData("guest", "Guest")]
        [InlineData("Silver", "Silver")]
        [InlineData("Gold", "Gold")]
        [InlineData("Platinum", "Platinum")]
        [InlineData("Admin", "Admin")]
        public void RoleDesciptionExistsForRole(string roleName, string description)
        {
            var actual = RoleDescriptions.GetDescription(roleName);
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains(description, actual);
        }
    }
}
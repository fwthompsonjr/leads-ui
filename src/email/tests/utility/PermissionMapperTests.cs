using legallead.email.models;
using legallead.email.utility;
using Microsoft.AspNetCore.Mvc;

namespace legallead.email.tests.utility
{
    public class PermissionMapperTests
    {
        [Theory]
        [InlineData("Discount")]
        [InlineData("PermissionLevel")]
        [InlineData("NotMapped")]
        public void MapperCanMapObject(string changeType)
        {
            var data = PermissionMockInfrastructure.GetResult(200, changeType);
            if (data is not OkObjectResult result) { return; }
            var converted = PermissionMapper.Mapper.Map<PermissionChangeResponse>(result);
            Assert.NotNull(converted);
        }

        [Theory]
        [InlineData("Discount")]
        [InlineData("Discount", false)]
        [InlineData("Discount", true, false)]
        [InlineData("Discount", true, true, true, false)]
        [InlineData("PermissionLevel")]
        [InlineData("PermissionLevel", false)]
        [InlineData("PermissionLevel", true, true, false)]
        [InlineData("PermissionLevel", true, true, true, false)]
        public void MapperCanValidateObject(string changeType, 
            bool hasEmail = true,
            bool hasDiscount = true,
            bool hasLevel = true,
            bool hasName = true)
        {
            var data = PermissionMockInfrastructure.GetResult(200, changeType);
            if (data is not OkObjectResult result) { return; }
            var converted = PermissionMapper.Mapper.Map<PermissionChangeResponse>(result);
            Assert.NotNull(converted);
            if (!hasName) { converted.Name = string.Empty; }
            if (!hasEmail) { converted.Email = string.Empty; }
            if (!hasDiscount) { converted.DiscountRequest = null; }
            if (!hasLevel) { converted.LevelRequest = null; }
            var validation = PermissionMapper.Mapper.Map<PermissionChangeValidation>(converted);
            if (!hasName || !hasEmail || !hasDiscount || !hasLevel) Assert.False(validation.IsValid);
            else Assert.True(validation.IsValid);
        }
    }
}
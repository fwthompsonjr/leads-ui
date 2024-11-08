using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace permissions.api.tests.Contollers
{
    public class AppControllerTests : BaseControllerTest
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<AppController>();
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData("dallas", false)]
        [InlineData("denton", true)]
        public void ControllerCanGetCounty(string name, bool expected)
        {

            var sut = GetProvider().GetRequiredService<AppController>();
            var response = sut.GetCounty(new() { Name = name, UserId = "default" });
            if (response is not OkObjectResult result)
            {
                Assert.Fail("Controller response not matched to expected.");
                return;
            }
            if (result.Value is not AuthorizedCountyModel model)
            {
                Assert.Fail("Controller response not matched to expected type.");
                return;
            }
            Assert.Equal(expected, string.IsNullOrEmpty(model.Name));
            Assert.Equal(expected, string.IsNullOrEmpty(model.Code));
        }

    }
}
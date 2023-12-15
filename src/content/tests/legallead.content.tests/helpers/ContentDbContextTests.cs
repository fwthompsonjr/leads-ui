using legallead.content.helpers;
using legallead.content.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.content.tests.helpers
{
    public class ContentDbContextTests
    {
        [Fact]
        public void ContextCanBeCreated()
        {
            var provider = TestContextProvider.GetTestFramework();
            var command = provider.GetRequiredService<IContentDbCommand>();
            var sut = new ContentDbContext(command);
            Assert.NotNull(sut);
        }

        [Fact]
        public void ContextCanGetCommand()
        {
            var provider = TestContextProvider.GetTestFramework();
            var command = provider.GetRequiredService<IContentDbCommand>();
            var sut = new ContentDbContext(command);
            Assert.NotNull(sut.GetCommand);
        }

        [Fact]
        public void ContextCanGetConnection()
        {
            var provider = TestContextProvider.GetTestFramework();
            var command = provider.GetRequiredService<IContentDbCommand>();
            var sut = new ContentDbContext(command);
            var connected = sut.CreateConnection();
            Assert.NotNull(connected);
        }
    }
}
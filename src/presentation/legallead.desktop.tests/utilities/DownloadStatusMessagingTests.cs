using Bogus;
using legallead.desktop.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.tests.utilities
{
    public class DownloadStatusMessagingTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(206)]
        [InlineData(400)]
        [InlineData(401)]
        [InlineData(402)]
        [InlineData(422)]
        public void SutCanGetStatusMessage(int status)
        {
            var description = new Faker().Lorem.Sentence(12);
            var message = DownloadStatusMessaging.GetMessage(status, description);
            Assert.NotEmpty(message);
        }
    }
}

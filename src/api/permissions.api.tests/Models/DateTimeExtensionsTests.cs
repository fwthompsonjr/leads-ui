using legallead.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Models
{
    public class DateTimeExtensionsTests
    {

        [Theory]
        [InlineData(false, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void DateTimeCanConvert(bool hasValue, bool isLocal)
        {
            var fakeDate = new Faker().Date.Recent();
            var exception = Record.Exception(() =>
            {
                DateTime? date = hasValue ? fakeDate : null;
                var dateKind = isLocal ? DateTimeKind.Local : DateTimeKind.Utc;
                _ = date.ToUnixTime(dateKind);
            });
            Assert.Null(exception);

        }
    }
}

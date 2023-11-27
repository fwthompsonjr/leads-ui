using legallead.jdbc.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.tests
{
    public class DataContextTests
    {
        [Fact]
        public void DataContextCanConstruct()
        {
            var db = new DataContext();
            Assert.NotNull(db);
        }

        [Fact]
        public async Task DataContextCanInit()
        {
            var exception = await Record.ExceptionAsync(async () => {
                var db = new DataContext();
                await db.Init();
            });
            Assert.Null(exception);
        }
    }
}

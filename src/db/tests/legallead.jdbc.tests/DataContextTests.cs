using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Moq;

namespace legallead.jdbc.tests
{
    public class DataContextTests
    {
        [Fact]
        public void DataContextCanConstruct()
        {
            var db = new DataContext(new Mock<IDapperCommand>().Object);
            Assert.NotNull(db);
        }

        [Fact]
        public async Task DataContextCanInit()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var db = new DataContext(new Mock<IDapperCommand>().Object);
                await db.Init();
            });
            Assert.Null(exception);
        }
    }
}
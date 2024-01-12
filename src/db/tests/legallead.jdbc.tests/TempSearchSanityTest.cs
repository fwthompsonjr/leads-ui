using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using System.Text;

namespace legallead.jdbc.tests
{
    public class SearchSanityTest
    {
        [Fact]
        public void SearchCanBeConstructed()
        {
            var exception = Record.Exception(() =>
            {
                var cmmd = new DapperExecutor();
                var db = new DataContext(cmmd);
                _ = new UserSearchRepository(db);
            });
            Assert.Null(exception);
        }
        [Fact]
        public async Task SearchCanGetHistory()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var cmmd = new DapperExecutor();
                var db = new DataContext(cmmd);
                var repo = new UserSearchRepository(db);
                await repo.History(Guid.NewGuid().ToString());
            });
            Assert.Null(exception);

        }
        [Theory]
        [InlineData(SearchTargetTypes.Detail)]
        [InlineData(SearchTargetTypes.Request)]
        [InlineData(SearchTargetTypes.Response)]
        public async Task SearchCanAppend(SearchTargetTypes target)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var cmmd = new DapperExecutor();
                var dat = Encoding.UTF8.GetBytes("This is a simple string");
                var db = new DataContext(cmmd);
                var repo = new UserSearchRepository(db);
                var response = await repo.Append(target, Guid.NewGuid().ToString(), dat);
                Assert.False(response.Key);
                Assert.Contains("FOREIGN KEY", response.Value);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task SearchCanAppendStatus()
        {
            SearchTargetTypes target = SearchTargetTypes.Status;
            var exception = await Record.ExceptionAsync(async () =>
            {
                var cmmd = new DapperExecutor();
                var dat = "This is a simple string";
                var db = new DataContext(cmmd);
                var repo = new UserSearchRepository(db);
                var response = await repo.Append(target, Guid.NewGuid().ToString(), dat);
                Assert.False(response.Key);
                Assert.Contains("FOREIGN KEY", response.Value);
            });
            Assert.Null(exception);
        }
        [Fact]
        public async Task SearchCanBegin()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var cmmd = new DapperExecutor();
                var dat = "This is a simple string";
                var db = new DataContext(cmmd);
                var repo = new UserSearchRepository(db);
                var response = await repo.Begin(Guid.NewGuid().ToString(), dat);
                Assert.False(response.Key);
                Assert.Contains("FOREIGN KEY", response.Value);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task SearchCanComplete()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var cmmd = new DapperExecutor();
                var db = new DataContext(cmmd);
                var repo = new UserSearchRepository(db);
                var response = await repo.Complete(Guid.NewGuid().ToString());
                Assert.True(response.Key);
                Assert.Contains("Record update completed.", response.Value);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task SearchCanUpdateRowCount()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var cmmd = new DapperExecutor();
                var db = new DataContext(cmmd);
                var repo = new UserSearchRepository(db);
                var response = await repo.UpdateRowCount(Guid.NewGuid().ToString(), 100);
                Assert.True(response.Key);
                Assert.Contains("Record update completed.", response.Value);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(SearchTargetTypes.Detail)]
        [InlineData(SearchTargetTypes.Request)]
        [InlineData(SearchTargetTypes.Response)]
        [InlineData(SearchTargetTypes.Status)]
        public async Task SearchCanGetTargets(SearchTargetTypes target)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var cmmd = new DapperExecutor();
                var db = new DataContext(cmmd);
                var repo = new UserSearchRepository(db);
                var response = await repo.GetTargets(target, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                Assert.NotNull(response);
            });
            Assert.Null(exception);
        }
    }
}

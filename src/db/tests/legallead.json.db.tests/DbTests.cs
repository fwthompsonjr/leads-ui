using legallead.json.db.entity;
using legallead.json.db.interfaces;
using legallead.json.db.tests.sample;
using legallead.json.tests;
using Moq;
using System.Reflection;

namespace legallead.json.db.tests
{
    public class DbTests : IDisposable
    {
        private static readonly object locker = new();
        private bool disposedValue;
        private readonly DataProvider Provider;

        public DbTests()
        {
            Provider = GetDataProvider();
        }

        [Fact]
        public void Db_Can_Add_Update_And_Delete()
        {
            var user = new UserEntity
            {
                Name = "Test",
                UserId = "john.smith@email.org",
                Pwd = "abcdefghijklmop"
            };
            Provider.Insert(user);
            Assert.NotNull(user.Id);
            user.Name = "Test Changed";
            Provider.Update(user);
            var item = Provider.FirstOrDefault(user, x => { return x.Id == user.Id; });
            Assert.NotNull(item);
            Assert.Equal(user.Name, item.Name);
            Provider.Delete(user);
        }

        [Fact]
        public void Db_Can_Add_And_FindAll()
        {
            var users = new List<UserEntity> {
                new UserEntity
                {
                    Name = "Test 1",
                    UserId = "test1.smith@email.org",
                    Pwd = "abcdefghijklmop"
                },
                new UserEntity
                {
                    Name = "Test 2",
                    UserId = "roger.smith@email.org",
                    Pwd = "abcdefghijklmop"
                },
                new UserEntity
                {
                    Name = "Test 3",
                    UserId = "test1.smith@email.org",
                    Pwd = "abcdefghijklmop"
                }
            };
            users.ForEach(u => Provider.Insert(u));

            var items = Provider.Where(users[0], x => { return x.UserId == users[0].UserId; });
            Assert.NotNull(items);
            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void Data_Can_Add_Update_And_Delete()
        {
            var fruit = new FruitEntity
            {
                Name = "Banana"
            };
            Provider.Insert(fruit);
            Assert.NotNull(fruit.Id);
            fruit.Name = "Test Changed";
            Provider.Update(fruit);
            var item = Provider.FirstOrDefault(fruit, x => { return x.Id == fruit.Id; });
            Assert.NotNull(item);
            Assert.Equal(fruit.Name, item.Name);
            Provider.Delete(fruit);
        }

        [Fact]
        public void Data_Can_Add_And_FindAll()
        {
            var fruits = new List<FruitEntity> {
                new FruitEntity
                {
                    Name = "Apple",
                },
                new FruitEntity
                {
                    Name = "Orange",
                },
                new FruitEntity
                {
                    Name = "Apple", // the name of this object is duplicate on purpose
                }
            };
            fruits.ForEach(u => Provider.Insert(u));

            var items = Provider.Where(fruits[0], x => { return x.Name == fruits[0].Name; });
            Assert.NotNull(items);
            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void UsStateInitNoItems()
        {
            const UsState? usState = null;
            var usResponse = new UsState();
            var mock = new Mock<IDataProvider>();
            mock.Setup(m =>
                m.FirstOrDefault(It.IsAny<UsState>(), It.IsAny<Func<UsState, bool>>())).Returns(usState);
            mock.Setup(m => m.Insert(It.IsAny<UsState>())).Returns(usResponse);

            var exception = Record.Exception(() =>
            {
                UsState.Initialize(mock.Object);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UsStateInitExistingItems()
        {
            var usState = new UsState();
            var mock = new Mock<IDataProvider>();
            mock.Setup(m =>
                m.FirstOrDefault(It.IsAny<UsState>(), It.IsAny<Func<UsState, bool>>())).Returns(usState);
            mock.Setup(m => m.Insert(It.IsAny<UsState>())).Throws(new Exception());

            var exception = Record.Exception(() =>
            {
                UsState.Initialize(mock.Object);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UsStateCountyInitNoItems()
        {
            const UsStateCounty? usState = null;
            var usResponse = new UsStateCounty();
            var mock = new Mock<IDataProvider>();
            mock.Setup(m =>
                m.FirstOrDefault(It.IsAny<UsStateCounty>(), It.IsAny<Func<UsStateCounty, bool>>())).Returns(usState);
            mock.Setup(m => m.Insert(It.IsAny<UsStateCounty>())).Returns(usResponse);

            var exception = Record.Exception(() =>
            {
                UsStateCounty.Initialize(mock.Object);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UsStateCountyInitExistingItems()
        {
            var usState = new UsStateCounty();
            var mock = new Mock<IDataProvider>();
            mock.Setup(m =>
                m.FirstOrDefault(It.IsAny<UsStateCounty>(), It.IsAny<Func<UsStateCounty, bool>>())).Returns(usState);
            mock.Setup(m => m.Insert(It.IsAny<UsStateCounty>())).Throws(new Exception());

            var exception = Record.Exception(() =>
            {
                UsStateCounty.Initialize(mock.Object);
            });
            Assert.Null(exception);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DropDb();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private static DataProvider GetDataProvider()
        {
            return new DataProvider();
        }

        private static void DropDb()
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null || assembly.Location == null) { return; }
            var execName = new Uri(assembly.Location).AbsolutePath;
            if (execName == null || !File.Exists(execName)) { return; }

            var contentRoot = Path.GetDirectoryName(execName) ?? "";
            var dataRoot = Path.Combine(contentRoot, "_db");
            if (!Directory.Exists(dataRoot)) { return; }
            var files = Directory.GetFiles(dataRoot, "*.json");

            lock (locker)
            {
                foreach (var file in files) { File.Delete(file); }
            }
        }
    }
}
using Bogus;
using legallead.permissions.api;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;

namespace permissions.api.tests
{
    public class GetObjectFromHeaderTests
    {
        private static readonly Faker faker = new();

        [Fact]
        public void CanGetObjectFromHeader()
        {
            const string headerName = "Test-Object";
            var obj = new ApplicationRequestModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Name.FullName()
            };
            var serialObj = JsonConvert.SerializeObject(obj);
            var headers = new HeaderDictionary
            {
                { "Content-Type", "application/json" },
                { headerName, serialObj }
            };
            var mock = GetMock();
            mock.SetupGet(x => x.Headers).Returns(headers);
            var request = mock.Object;
            var response = request.GetObjectFromHeader<ApplicationRequestModel>(headerName);
            Assert.NotNull(response);
            Assert.NotNull(response.Id);
            Assert.Equal(obj.Id, response.Id);
            Assert.Equal(obj.Name, response.Name);
        }

        [Fact]
        public void GetObjectWithMissingHeaderNameReturnsNull()
        {
            const string headerName = "Test-Object";
            var falseHeaderName = $"{headerName}-Missing";
            var obj = new ApplicationRequestModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Name.FullName()
            };
            var serialObj = JsonConvert.SerializeObject(obj);
            var headers = new HeaderDictionary
            {
                { "Content-Type", "application/json" },
                { headerName, serialObj }
            };
            var mock = GetMock();
            mock.SetupGet(x => x.Headers).Returns(headers);
            var request = mock.Object;
            var response = request.GetObjectFromHeader<ApplicationRequestModel>(falseHeaderName);
            Assert.Null(response);
        }

        [Fact]
        public void GetObjectOnExceptionReturnsNull()
        {
            const string headerName = "Test-Object";
            var mock = GetMock();
            mock.SetupGet(x => x.Headers).Throws(new ApplicationException());
            var request = mock.Object;
            var response = request.GetObjectFromHeader<ApplicationRequestModel>(headerName);
            Assert.Null(response);
        }

        [Fact]
        public void GetObjectWithEmptyHeaderNameReturnsNull()
        {
            const string headerName = "";
            var mock = GetMock();
            var request = mock.Object;
            var response = request.GetObjectFromHeader<ApplicationRequestModel>(headerName);
            Assert.Null(response);
        }

        [Fact]
        public void GetObjectWithMissingHeaderValueReturnsNull()
        {
            const string headerName = "Test-Object";
            var headers = new HeaderDictionary
            {
                { "Content-Type", "application/json" },
                { headerName, "" }
            };
            var mock = GetMock();
            mock.SetupGet(x => x.Headers).Returns(headers);
            var request = mock.Object;
            var response = request.GetObjectFromHeader<ApplicationRequestModel>(headerName);
            Assert.Null(response);
        }

        private static Mock<HttpRequest> GetMock()
        {
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));
            return request;
        }
    }
}

using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using legallead.permissions.api;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Services
{
    public class DbHistoryServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var provider = GetDbProvider();
            var service = provider.GetRequiredService<IDbHistoryService>();
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ServiceCanBeginAsync(int responseId)
        {
            var provider = GetDbProvider();
            var service = provider.GetRequiredService<IDbHistoryService>();
            var request = beginfaker.Generate();
            var response = responseId == 0 ? null : historyfaker.Generate();
            var mock = provider.GetRequiredService<Mock<IDbHistoryRepository>>();
            mock.Setup(m => m.BeginAsync(It.IsAny<DbHistoryRequest>())).ReturnsAsync(response);
            await service.BeginAsync(request);
            mock.Verify(m => m.BeginAsync(It.IsAny<DbHistoryRequest>()));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ServiceCanCompleteAsync(int responseId)
        {
            var provider = GetDbProvider();
            var service = provider.GetRequiredService<IDbHistoryService>();
            var request = completefaker.Generate();
            var response = responseId == 0 ? null : historyfaker.Generate();
            var mock = provider.GetRequiredService<Mock<IDbHistoryRepository>>();
            mock.Setup(m => m.CompleteAsync(It.IsAny<DbHistoryRequest>())).ReturnsAsync(response);
            await service.CompleteAsync(request);
            mock.Verify(m => m.CompleteAsync(It.IsAny<DbHistoryRequest>()));
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public async Task ServiceCanFindAsync(int responseId)
        {
            var provider = GetDbProvider();
            var service = provider.GetRequiredService<IDbHistoryService>();
            var request = new Faker().Random.Guid().ToString();
            var response = responseId == -1 ? null : resultfaker.Generate(responseId);
            var mock = provider.GetRequiredService<Mock<IDbHistoryRepository>>();
            mock.Setup(m => m.FindAsync(It.IsAny<string>())).ReturnsAsync(response);
            await service.FindAsync(new FindDataRequest { Id = request });
            mock.Verify(m => m.FindAsync(It.IsAny<string>()));
        }



        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public async Task ServiceCanUploadAsync(int responseId)
        {
            var fkr = new Faker();
            var provider = GetDbProvider();
            var service = provider.GetRequiredService<IDbHistoryService>();
            var requestid = fkr.Random.Guid().ToString();
            var data = uploadfaker.Generate(responseId);
            var request = new UploadDataRequest { Id = requestid, Contents = data };
            var response = fkr.Random.Bool();
            var mock = provider.GetRequiredService<Mock<IDbHistoryRepository>>();
            mock.Setup(m => m.UploadAsync(It.IsAny<DbUploadRequest>())).ReturnsAsync(response);
            await service.UploadAsync(request);
            mock.Verify(m => m.UploadAsync(It.IsAny<DbUploadRequest>()));
        }

        [Fact]
        public void MapperCanMapBeqinRequest()
        {
            var faker = GetFaker<BeginDataRequest>();
            if (faker == null) Assert.Fail();
            var obj = faker.Generate();
            var mapped = ModelMapper.Mapper.Map<DbHistoryRequest>(obj);
            Assert.NotNull(mapped);
            Assert.Equal(obj.SearchDate, mapped.SearchDate);
            Assert.Equal(obj.CountyId, mapped.CountyId);
            Assert.Equal(obj.SearchTypeId, mapped.SearchTypeId);
            Assert.Equal(obj.CaseTypeId, mapped.CaseTypeId);
            Assert.Equal(obj.DistrictCourtId, mapped.DistrictCourtId);
            Assert.Equal(obj.DistrictSearchTypeId, mapped.DistrictSearchTypeId);
        }

        [Fact]
        public void MapperCanMapCompleteRequest()
        {
            var faker = GetFaker<CompleteDataRequest>();
            if (faker == null) Assert.Fail();
            var obj = faker.Generate();
            var mapped = ModelMapper.Mapper.Map<DbHistoryRequest>(obj);
            Assert.NotNull(mapped);
            Assert.Equal(obj.SearchDate, mapped.SearchDate);
            Assert.Equal(obj.CountyId, mapped.CountyId);
            Assert.Equal(obj.SearchTypeId, mapped.SearchTypeId);
            Assert.Equal(obj.CaseTypeId, mapped.CaseTypeId);
            Assert.Equal(obj.DistrictCourtId, mapped.DistrictCourtId);
            Assert.Equal(obj.DistrictSearchTypeId, mapped.DistrictSearchTypeId);
        }

        [Fact]
        public void MapperCanMapFindRequest()
        {
            var faker = GetFaker<FindRequestResponse>();
            if (faker == null) Assert.Fail();
            var obj = faker.Generate();
            var mapped = ModelMapper.Mapper.Map<DbSearchHistoryResultBo>(obj);
            Assert.NotNull(mapped);
            Assert.Equal(obj.Name, mapped.Name);
            Assert.Equal(obj.Zip, mapped.Zip);
            Assert.Equal(obj.Address1, mapped.Address1);
            Assert.Equal(obj.Address2, mapped.Address2);
            Assert.Equal(obj.Address3, mapped.Address3);
            Assert.Equal(obj.CaseNumber, mapped.CaseNumber);
            Assert.Equal(obj.Court, mapped.Court);
            Assert.Equal(obj.CaseType, mapped.CaseType);
            Assert.Equal(obj.Plaintiff, mapped.Plaintiff);
        }

        [Fact]
        public void MapperCanMapUploadRequest()
        {
            var faker = GetFaker<UploadHistoryItem>();
            if (faker == null) Assert.Fail();
            var obj = faker.Generate();
            var mapped = ModelMapper.Mapper.Map<DbSearchHistoryResultBo>(obj);
            Assert.NotNull(mapped);
            Assert.Equal(obj.Name, mapped.Name);
            Assert.Equal(obj.Zip, mapped.Zip);
            Assert.Equal(obj.Address1, mapped.Address1);
            Assert.Equal(obj.Address2, mapped.Address2);
            Assert.Equal(obj.Address3, mapped.Address3);
            Assert.Equal(obj.CaseNumber, mapped.CaseNumber);
            Assert.Equal(obj.Court, mapped.Court);
            Assert.Equal(obj.CaseType, mapped.CaseType);
            Assert.Equal(obj.Plaintiff, mapped.Plaintiff);
        }
        [Fact]
        public void MapperCanMapFindResponse()
        {
            var src = resultfaker.Generate();
            var dst = ModelMapper.Mapper.Map<FindRequestResponse>(src);
            Assert.NotNull(dst);
        }

        private static ServiceProvider GetDbProvider()
        {
            var services = new ServiceCollection();
            var mock = new Mock<IDbHistoryRepository>();
            services.AddSingleton(mock);
            services.AddSingleton(mock.Object);
            services.AddSingleton<IDbHistoryService, DbHistoryService>();
            return services.BuildServiceProvider();
        }

        private static Faker<T>? GetFaker<T>() where T : class
        {
            var type = typeof(T);
            if (type == typeof(BeginDataRequest)) return beginfaker as Faker<T>;
            if (type == typeof(CompleteDataRequest)) return completefaker as Faker<T>;
            if (type == typeof(FindRequestResponse)) return findfaker as Faker<T>;
            if (type == typeof(UploadHistoryItem)) return uploadfaker as Faker<T>;
            return null;
        }

        private static readonly Faker<BeginDataRequest>
            beginfaker = new Faker<BeginDataRequest>()
            .RuleFor(x => x.SearchDate, y => y.Date.Past())
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.SearchTypeId, y => y.Random.Int(1, 50))
            .RuleFor(x => x.CaseTypeId, y => y.Random.Int(1, 50))
            .RuleFor(x => x.DistrictCourtId, y => y.Random.Int(1, 50))
            .RuleFor(x => x.DistrictSearchTypeId, y => y.Random.Int(1, 50));

        private static readonly Faker<CompleteDataRequest>
            completefaker = new Faker<CompleteDataRequest>()
            .RuleFor(x => x.SearchDate, y => y.Date.Past())
            .RuleFor(x => x.CountyId, y => y.Random.Int(0, 1000))
            .RuleFor(x => x.SearchTypeId, y => y.Random.Int(0, 50))
            .RuleFor(x => x.CaseTypeId, y => y.Random.Int(0, 50))
            .RuleFor(x => x.DistrictCourtId, y => y.Random.Int(0, 50))
            .RuleFor(x => x.DistrictSearchTypeId, y => y.Random.Int(0, 50));

        private static readonly Faker<FindRequestResponse>
            findfaker = new Faker<FindRequestResponse>()
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.Zip, y => y.Address.ZipCode())
            .RuleFor(x => x.Address1, y => y.Address.StreetAddress(true))
            .RuleFor(x => x.Address2, y =>
            {
                var hasValue = y.Random.Bool();
                if (!hasValue) return string.Empty;
                return y.Address.SecondaryAddress();
            })
            .RuleFor(x => x.Address3, y =>
            {
                var addr = $"{y.Address.City()}, {y.Address.StateAbbr()}";
                return addr;
            })
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.Court, y => y.Random.AlphaNumeric(5))
            .RuleFor(x => x.CaseType, y => y.Random.AlphaNumeric(5))
            .RuleFor(x => x.CaseStyle, y => y.Lorem.Sentence(7))
            .RuleFor(x => x.Plaintiff, y => y.Person.FullName);

        private static readonly Faker<UploadHistoryItem>
            uploadfaker = new Faker<UploadHistoryItem>()
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.Zip, y => y.Address.ZipCode())
            .RuleFor(x => x.Address1, y => y.Address.StreetAddress(true))
            .RuleFor(x => x.Address2, y =>
            {
                var hasValue = y.Random.Bool();
                if (!hasValue) return string.Empty;
                return y.Address.SecondaryAddress();
            })
            .RuleFor(x => x.Address3, y =>
            {
                var addr = $"{y.Address.City()}, {y.Address.StateAbbr()}";
                return addr;
            })
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.Court, y => y.Random.AlphaNumeric(5))
            .RuleFor(x => x.CaseType, y => y.Random.AlphaNumeric(5))
            .RuleFor(x => x.CaseStyle, y => y.Lorem.Sentence(7))
            .RuleFor(x => x.Plaintiff, y => y.Person.FullName);

        private static readonly Faker<DbSearchHistoryBo>
            historyfaker = new Faker<DbSearchHistoryBo>()
            .RuleFor(x => x.SearchDate, y => y.Date.Past())
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.SearchTypeId, y => y.Random.Int(1, 50))
            .RuleFor(x => x.CaseTypeId, y => y.Random.Int(1, 50))
            .RuleFor(x => x.DistrictCourtId, y => y.Random.Int(1, 50))
            .RuleFor(x => x.DistrictSearchTypeId, y => y.Random.Int(1, 50));

        private static readonly Faker<DbSearchHistoryResultBo>
            resultfaker = new Faker<DbSearchHistoryResultBo>()
            .FinishWith((fk, dest) =>
            {
                var src = uploadfaker.Generate();
                var tmp = ModelMapper.Mapper.Map<DbSearchHistoryResultBo>(src);
                tmp.Id = fk.Random.Guid().ToString();
                tmp.CreateDate = fk.Date.Recent();
                tmp.SearchHistoryId = fk.Random.Guid().ToString();
                dest = tmp;
            });
    }
}

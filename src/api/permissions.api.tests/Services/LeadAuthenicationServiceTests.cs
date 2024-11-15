using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Services;
using Newtonsoft.Json;

namespace permissions.api.tests.Services
{
    public class LeadAuthenicationServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var sut = new MockLeadAuthenicationService();
            Assert.NotNull(sut);
            Assert.NotNull(sut.Svc);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanChangeCountyCredentialAsync(bool isNew)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var fkr = new Faker();
                var request = new
                {
                    userId = fkr.Random.Guid().ToString("D"),
                    county = fkr.Address.County(),
                    userName = fkr.Person.UserName,
                    password = fkr.Random.AlphaNumeric(15)
                };
                var service = new MockLeadAuthenicationService();
                service.SetupCountyCredential(request.county, request.userName, request.password, isNew);
                // Act
                _ = await service.Svc.ChangeCountyCredentialAsync(
                    request.userId,
                    request.county,
                    request.userName,
                    request.password);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(true, "-1")]
        public async Task ServiceCanChangeCountyPermissionAsync(bool isNew, string countyType = "")
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var fkr = new Faker();
                var request = new
                {
                    userId = fkr.Random.Guid().ToString("D"),
                    county = string.IsNullOrEmpty(countyType) ? fkr.Address.County() : countyType
                };
                var service = new MockLeadAuthenicationService();
                service.SetupCountyPermission(request.county, isNew);
                // Act
                _ = await service.Svc.ChangeCountyPermissionAsync(
                    request.userId,
                    request.county);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(true, false)]
        public async Task ServiceCanChangePasswordAsync(bool isMatched, bool verificationResponse = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var fkr = new Faker();
                var request = new
                {
                    userId = fkr.Random.Guid().ToString("D"),
                    userName = fkr.Person.UserName,
                    oldpassword = fkr.Random.AlphaNumeric(15),
                    newpassword = fkr.Random.AlphaNumeric(20),
                };
                var service = new MockLeadAuthenicationService();
                service.SetupPassword(
                    request.userName,
                    request.oldpassword,
                    request.newpassword,
                    isMatched,
                    verificationResponse);
                // Act
                _ = await service.Svc.ChangePasswordAsync(
                    request.userId,
                    request.oldpassword,
                    request.newpassword);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanCreateLoginAsync(bool isCreated)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var fkr = new Faker();
                var request = new
                {
                    userId = fkr.Random.Guid().ToString("D"),
                    userName = fkr.Person.UserName,
                    password = fkr.Random.AlphaNumeric(15)
                };
                var service = new MockLeadAuthenicationService();
                service.SetupLogin(request.userId, isCreated);
                // Act
                _ = await service.Svc.CreateLoginAsync(
                    request.userName,
                    request.password);
            });
            Assert.Null(error);
        }

        private sealed class MockLeadAuthenicationService
        {
            private Mock<ILeadUserRepository> MqRepo { get; } = new();
            private ILeadSecurityService LeadSvc { get; } = new LeadSecurityService();
            public LeadAuthenicationService Svc => _service;
            public MockLeadAuthenicationService()
            {
                _service = new LeadAuthenicationService(MqRepo.Object, LeadSvc);
            }
            public void SetupCountyCredential(string county, string userName, string password, bool ismissing = false)
            {
                var bo = LeadUserBoGenerator.GetBo(1, 1);
                var current = bo.CountyData.ToInstance<List<LeadUserCountyDto>>();
                if (!ismissing && current != null)
                {
                    var dto = current[0];
                    var credentials = $"{userName}|{password}";
                    var obj = LeadSvc.CreateSecurityModel(credentials);
                    dto.CountyName = county;
                    dto.Phrase = obj.Phrase;
                    dto.Vector = obj.Vector;
                    dto.Token = obj.Token;
                    bo.CountyData = JsonConvert.SerializeObject(current);
                }
                MqRepo.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(bo);
                MqRepo.Setup(x => x.GetUserById(It.IsAny<string>())).ReturnsAsync(bo);
            }


            public void SetupCountyPermission(string countylist, bool ismissing = false)
            {
                var bo = LeadUserBoGenerator.GetBo(1, 1);
                var current = bo.IndexData.ToInstance<List<LeadUserCountyIndexDto>>();
                if (!ismissing && current != null)
                {
                    var dto = current[0];
                    dto.CountyList = countylist;
                    bo.IndexData = JsonConvert.SerializeObject(current);
                }
                MqRepo.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(bo);
                MqRepo.Setup(x => x.GetUserById(It.IsAny<string>())).ReturnsAsync(bo);
            }

            public void SetupPassword(string userId, string oldpassword, string newpassword, bool ismissing = false, bool apiResponse = true)
            {
                var bo1 = LeadUserBoGenerator.GetBo(1, 1);
                var bo2 = LeadUserBoGenerator.GetBo(1, 1);
                var obj = new List<LeadUserBo> { bo1, bo2 };
                obj.ForEach(bo =>
                {
                    var id = obj.IndexOf(bo);
                    var pwd = id == 0 ? oldpassword : newpassword;
                    var credentials = $"{userId}|{pwd}";
                    var model = LeadSvc.CreateSecurityModel(credentials);
                    var current = bo.UserData.ToInstance<LeadUserDto>();
                    if (!ismissing && current != null)
                    {
                        current.Token = model.Token;
                        current.Vector = model.Vector;
                        current.Phrase = model.Phrase;
                        bo.UserData = JsonConvert.SerializeObject(current);
                    }
                });
                MqRepo.Setup(x => x.UpdateAccount(It.IsAny<LeadUserDto>()))
                    .ReturnsAsync(apiResponse);
                MqRepo.SetupSequence(x => x.GetUser(It.IsAny<string>()))
                    .ReturnsAsync(bo1)
                    .ReturnsAsync(bo2);
                MqRepo.SetupSequence(x => x.GetUserById(It.IsAny<string>()))
                    .ReturnsAsync(bo1)
                    .ReturnsAsync(bo2);
            }
            public void SetupLogin(string userId, bool apiResponse = true)
            {
                var addResponse = apiResponse ? userId : string.Empty;
                MqRepo.Setup(x => x.AddAccount(It.IsAny<LeadUserDto>()))
                    .ReturnsAsync(addResponse);
            }
            private readonly LeadAuthenicationService _service;
        }
    }
}

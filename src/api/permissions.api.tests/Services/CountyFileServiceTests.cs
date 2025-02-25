using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Services
{
    public class CountyFileServiceTests
    {

        [Fact]
        public void ServiceCanBeCreated()
        {
            var provider = GetDbProvider();
            var service = provider.GetRequiredService<ICountyFileService>();
            Assert.NotNull(service);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ServiceCanGetAsync(int responseId)
        {
            var provider = GetDbProvider();
            var error = provider.GetRequiredService<Exception>();
            var service = provider.GetRequiredService<ICountyFileService>();
            var request = modelfaker.Generate();
            var response = responseId == 0 ? null : modelfaker.Generate();
            var mock = provider.GetRequiredService<Mock<ICountyFileRepository>>();
            if (responseId == 2)
            {
                mock.Setup(m => m.GetContentAsync(It.IsAny<DbCountyFileModel>())).ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.GetContentAsync(It.IsAny<DbCountyFileModel>())).ReturnsAsync(response);
            }
            await service.GetAsync(request);
            mock.Verify(m => m.GetContentAsync(It.IsAny<DbCountyFileModel>()));
        }

        [Theory]
        [InlineData(-1, -1)] // all services return good result
        [InlineData(0, -1)] // service 0 returns negative result
        [InlineData(1, -1)] // service 1 returns negative result
        [InlineData(2, -1)] // service 2 returns negative result
        [InlineData(-1, 0)] // service 0 throws exception
        [InlineData(-1, 1)] // service 1 throws exception
        [InlineData(-1, 2)] // service 1 throws exception
        public async Task ServiceCanSaveAsync(int responseId, int errorId)
        {
            var provider = GetDbProvider();
            var error = provider.GetRequiredService<Exception>();
            var service = provider.GetRequiredService<ICountyFileService>();
            var request = modelfaker.Generate();
            var responses = new KeyValuePair<bool, string>[] { new(true, ""), new(false, "mocked negative result") };
            var mock = provider.GetRequiredService<Mock<ICountyFileRepository>>();
            if (errorId == -1)
            {
                for (var s = 0; s < 3; s++)
                {
                    var serviceResponse = (responseId == s) ? responses[1] : responses[0];
                    if (s == 0)
                        mock.Setup(m => m.UpdateTypeAsync(It.IsAny<DbCountyFileModel>())).ReturnsAsync(serviceResponse);
                    if (s == 1)
                        mock.Setup(m => m.UpdateStatusAsync(It.IsAny<DbCountyFileModel>())).ReturnsAsync(serviceResponse);
                    if (s == 2)
                        mock.Setup(m => m.UpdateContentAsync(It.IsAny<DbCountyFileModel>())).ReturnsAsync(serviceResponse);
                }
            }
            else
            {
                if (errorId == 0)
                {
                    mock.Setup(m => m.UpdateTypeAsync(It.IsAny<DbCountyFileModel>())).ThrowsAsync(error);
                }
                else
                {
                    mock.Setup(m => m.UpdateTypeAsync(It.IsAny<DbCountyFileModel>())).ReturnsAsync(responses[0]);
                }
                if (errorId == 1)
                {
                    mock.Setup(m => m.UpdateStatusAsync(It.IsAny<DbCountyFileModel>())).ThrowsAsync(error);
                }
                else
                {
                    mock.Setup(m => m.UpdateStatusAsync(It.IsAny<DbCountyFileModel>())).ReturnsAsync(responses[0]);
                }
                if (errorId == 2)
                {
                    mock.Setup(m => m.UpdateContentAsync(It.IsAny<DbCountyFileModel>())).ThrowsAsync(error);
                }
                else
                {
                    mock.Setup(m => m.UpdateContentAsync(It.IsAny<DbCountyFileModel>())).ReturnsAsync(responses[0]);
                }
            }
            await service.SaveAsync(request);
            mock.Verify(m => m.UpdateTypeAsync(It.IsAny<DbCountyFileModel>()));
            mock.Verify(m => m.UpdateStatusAsync(It.IsAny<DbCountyFileModel>()));
            mock.Verify(m => m.UpdateContentAsync(It.IsAny<DbCountyFileModel>()));
        }

        private static ServiceProvider GetDbProvider()
        {
            var services = new ServiceCollection();
            var mock = new Mock<ICountyFileRepository>();
            var error = new Faker().System.Exception();
            services.AddSingleton(error);
            services.AddSingleton(mock);
            services.AddSingleton(mock.Object);
            services.AddSingleton<ICountyFileService, CountyFileService>();
            return services.BuildServiceProvider();
        }

        private static readonly Faker<DbCountyFileModel> modelfaker = 
            new Faker<DbCountyFileModel>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.FileType, y => y.PickRandom(_fileTypes))
            .RuleFor(x => x.FileStatus, y => y.PickRandom(_fileStatus))
            .RuleFor(x => x.FileContent, y => y.Hacker.Phrase());

        private static readonly List<string> _fileTypes = new List<string> { "NONE", "EXL", "CSV", "JSON" };
        private static readonly List<string> _fileStatus = new List<string> { "EMPTY", "ENCODED", "DECODED", "DOWNLOADED" };
    }
}

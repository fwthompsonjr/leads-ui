using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Diagnostics;

namespace legallead.jdbc.tests.implementations
{
    public class HarrisCriminalZipFileReaderTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("not-an-existing-file-path")]
        public void ServiceReadRequiresFileName(string sourceFile)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using var service = GetReader(sourceFile);
                service.Read();
            });
        }

        [Theory]
        [InlineData(historyData)]
        public void ServiceCanReadZipContent(string sourceFile)
        {
            if (!Debugger.IsAttached) return;
            var error = Record.Exception(() =>
            {
                using var service = GetReader(sourceFile);
                service.Read();
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(historyData)]
        public void ServiceCanTranslateZipContent(string sourceFile)
        {
            if (!Debugger.IsAttached) return;
            var error = Record.Exception(() =>
            {
                using var service = GetReader(sourceFile);
                service.Translate();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(historyData)]
        public void ServiceCanTransferZipContent(string sourceFile)
        {
            if (!Debugger.IsAttached) return;
            var error = Record.Exception(() =>
            {
                using var service = GetReader(sourceFile);
                service.Transfer();
            });
            Assert.Null(error);
        }

        [Fact]
        public void LookupServiceCanMapData()
        {
            var error = Record.Exception(() =>
            {
                _ = HarrisLookupService.Data;
            });
            Assert.Null(error);
        }
        private static HarrisCriminalZipFileReader GetReader(string sourceFile)
        {
            var response = new KeyValuePair<bool, string>(true, "test");
            var completion = Task.FromResult(response);
            var mock = new Mock<IHarrisLoadRepository>();
            mock.Setup(x => x.Append(It.IsAny<string>())).Returns(completion);
            return new HarrisCriminalZipFileReader(sourceFile, mock.Object);
        }


        [Theory]
        [InlineData(historyData)]
        public void ServiceCanTransferRealTime(string sourceFile)
        {
            var svc = TheProvider.GetRequiredService<IHarrisLoadRepository>();
            if (!Debugger.IsAttached) return;
            var error = Record.Exception(() =>
            {
                using var service = new HarrisCriminalZipFileReader(sourceFile, svc);
                service.Transfer();
            });
            Assert.Null(error);
        }
        private static IServiceProvider TheProvider => _services ??= GetProvider();

        private static IServiceProvider? _services = null;
        private static ServiceProvider GetProvider()
        {
            const string environ = "Test";
            var services = new ServiceCollection();
            services.AddSingleton<IDataInitializer, DataInitializer>();
            services.AddSingleton<IDapperCommand, DapperExecutor>(); 
            services.AddScoped(d =>
            {
                var command = d.GetRequiredService<IDapperCommand>();
                var dbint = d.GetRequiredService<IDataInitializer>();
                return new DataContext(command, dbint, environ);
            });
            services.AddScoped<IHarrisLoadRepository, HarrisLoadRepository>();
            return services.BuildServiceProvider();
        }
        
        private const string historyData = "C:\\_d\\lead-old\\_notes\\Weekly_Historical_Criminal.zip";
    }
}

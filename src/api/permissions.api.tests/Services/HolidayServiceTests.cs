using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Services
{
    public class HolidayServiceTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public async Task ServiceVerifyIsHolidayAsync(int responseId)
        {
            var fkr = new Faker();
            var response = fkr.Random.Bool();
            var holidayDate = fkr.Date.Future().ToString("d");
            var request = responseId switch
            {
                0 => "",
                1 => "      ",
                2 => "not-valid-date",
                _ => holidayDate
            };
            var provider = GetDbProvider();
            var service = provider.GetRequiredService<IHolidayService>();
            var mock = provider.GetRequiredService<Mock<IHolidayRepository>>();
            mock.Setup(m => m.IsHolidayAsync(It.IsAny<DateTime>())).ReturnsAsync(response);
            var error = await Record.ExceptionAsync(async () =>
            {
                await service.IsHolidayAsync(request);
            });
            Assert.Null(error);

        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public async Task ServiceCanGetHolidaysAsync(int responseId)
        {
            var response = responseId < 0 ? null : bofaker.Generate(responseId);
            var provider = GetDbProvider();
            var service = provider.GetRequiredService<IHolidayService>();
            var mock = provider.GetRequiredService<Mock<IHolidayRepository>>();
            mock.Setup(m => m.GetHolidaysAsync()).ReturnsAsync(response);
            var error = await Record.ExceptionAsync(async () =>
            {
                await service.GetHolidaysAsync();
            });
            Assert.Null(error);
        }

        [Fact]
        public void MapperCanMapFindResponse()
        {
            var src = bofaker.Generate();
            var dst = ModelMapper.Mapper.Map<HolidayResponse>(src);
            Assert.NotNull(dst);
        }

        private static ServiceProvider GetDbProvider()
        {
            var services = new ServiceCollection();
            var mock = new Mock<IHolidayRepository>();
            services.AddSingleton(mock);
            services.AddSingleton(mock.Object);
            services.AddSingleton<IHolidayService, HolidayService>();
            return services.BuildServiceProvider();
        }


        private static readonly Faker<HoliDateBo>
            bofaker = new Faker<HoliDateBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.HoliDate, y => y.Date.Past())
            .RuleFor(x => x.CreateDate, y => y.Date.Past());

    }
}
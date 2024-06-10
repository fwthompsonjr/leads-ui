using legallead.installer.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.installer.tests
{
    public class CommandInstallVerificationTests : BaseParserTest
    {
        [Theory]
        [InlineData("", "", "")]
        [InlineData("", "application", "")]
        [InlineData("", "application", "not-a-number")]
        [InlineData("", "application", "100")]
        [InlineData("", "legallead.reader.service", "")]
        public async Task InstallCanExecuteParameters(string version, string name, string id)
        {
            var provider = GetProvider();
            var handler = provider.GetRequiredService<CommandHandler>();
            var problem = await Record.ExceptionAsync(async () =>
            {
                await handler.Install(version, name, id);
            });
            Assert.Null(problem);

        }
    }
}

using legallead.permissions.api.Custom;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Custom
{
    public class CustomExceptionTests
    {
        [Theory]
        [InlineData(typeof(InvoiceAmountMismatchedException))]
        [InlineData(typeof(InvoiceNotFoundException))]
        [InlineData(typeof(SubscriptionNotFoundException))]
        [InlineData(typeof(PaymentIntentNotFoundException))]
        public void ExceptionCanBeCreated(Type type)
        {
            var provider = GetServiceProvider();
            var error = Record.Exception(() =>
            {
                var actual = provider.GetService(type);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }

        private static ServiceProvider GetServiceProvider()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<InvoiceAmountMismatchedException>();
            collection.AddSingleton<InvoiceNotFoundException>();
            collection.AddSingleton<SubscriptionNotFoundException>();
            collection.AddSingleton<PaymentIntentNotFoundException>();
            return collection.BuildServiceProvider();
        }
    }
}

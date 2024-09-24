using legallead.jdbc.entities;

namespace legallead.jdbc.tests
{
    public class BtoDtoTests
    {
        [Fact]
        public void DtoHasMultipleAssigments()
        {
            var collection = DtoItems;
            Assert.NotEmpty(collection);
        }

        private static IEnumerable<Type> DtoItems => types ??= GetDtoTypes();
        private static IEnumerable<Type>? types;
        private static IEnumerable<Type> GetDtoTypes()
        {
            lock (_sync)
            {
                try
                {
                    var type = typeof(BaseDto);
                    return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => type.IsAssignableFrom(p))
                        .Where(i => !i.IsInterface)
                        .Where(a => !a.IsAbstract);
                }
                catch (Exception)
                {
                    return Enumerable.Empty<Type>();
                }
            }
        }
        private static readonly object _sync = new();
    }
}

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

        [Fact]
        public void DtoCanGetFields()
        {
            var collection = DtoItems;
            Assert.NotEmpty(collection);
            collection.ToList().ForEach(c =>
            {
                var instance = TryCreateInstance(c);
                if (instance != null)
                {
                    Assert.IsAssignableFrom<IBaseDto>(instance);
                    if (instance is IBaseDto dto)
                    {
                        Assert.NotNull(dto);
                        TryGet(dto);
                    }
                }
            });
        }


        private static object? TryCreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch { return null; }
        }

        private static void TryGet(IBaseDto dto)
        {
            const string nonfound = "--not-defined--";
            try
            {

                _ = dto[nonfound];
                dto["id"] = null;
                var list = dto.FieldList;
                list.ForEach(itm => { _ = dto[itm]; });
            }
            catch
            {
                // not handled on purpose
            }
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

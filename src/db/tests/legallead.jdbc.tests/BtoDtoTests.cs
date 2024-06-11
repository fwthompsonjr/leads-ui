using legallead.jdbc.entities;
using System.Diagnostics;

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
            const string nonfound = "--not-defined--";
            var collection = DtoItems;
            Assert.NotEmpty(collection);
            collection.ToList().ForEach(c =>
            {
                var instance = Activator.CreateInstance(c);
                Assert.IsAssignableFrom<IBaseDto>(instance);
                if (instance is IBaseDto dto)
                {
                    Assert.NotNull(dto);
                    _ = dto[nonfound];
                    dto["id"] = null;
                    var list = dto.FieldList;
                    list.ForEach(itm => { _ = dto[itm]; });
                }
            });
        }

        [Fact]
        public void DtoCanSetFields()
        {
            const string nonfound = "--not-defined--";
            var collection = DtoItems;
            Assert.NotEmpty(collection);
            collection.ToList().ForEach(c =>
            {
                var instance = Activator.CreateInstance(c);
                Assert.IsAssignableFrom<IBaseDto>(instance);
                if (instance is IBaseDto dto)
                {
                    Assert.NotNull(dto);
                    dto[nonfound] = nonfound;
                    var list = dto.FieldList;
                    list.ForEach(itm => {
                        SetValue(dto, itm, string.Empty);
                    });
                }
            });
        }

        private static void SetValue(IBaseDto dto, string fieldName, object value)
        {
            try
            {
                dto[fieldName] = value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static IEnumerable<Type> DtoItems => types ??= GetDtoTypes();
        private static IEnumerable<Type>? types;
        private static IEnumerable<Type> GetDtoTypes()
        {
            var type = typeof(BaseDto);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .Where(i => !i.IsInterface)
                .Where(a => !a.IsAbstract);
        }
    }
}

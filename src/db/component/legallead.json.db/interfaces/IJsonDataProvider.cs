using legallead.json.entities;
using legallead.json.interfaces;

namespace legallead.json.db.interfaces
{
    public interface IJsonDataProvider
    {
        T Delete<T>(T entity) where T : BaseEntity<T>, IDataEntity, new();

        T? FirstOrDefault<T>(T entity, Func<T, bool> expression) where T : BaseEntity<T>, IDataEntity, new();

        T? FirstOrDefault<T>(Func<T, bool> expression) where T : BaseEntity<T>, IDataEntity, new();

        T Insert<T>(T entity) where T : class, IDataEntity, new();

        T Update<T>(T entity) where T : BaseEntity<T>, IDataEntity, new();

        IEnumerable<T>? Where<T>(T entity, Func<T, bool> expression) where T : BaseEntity<T>, IDataEntity, new();

        IEnumerable<T>? Where<T>(Func<T, bool> expression) where T : BaseEntity<T>, IDataEntity, new();
    }
}
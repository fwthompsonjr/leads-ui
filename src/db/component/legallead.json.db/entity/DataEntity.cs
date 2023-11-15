using legallead.json.entities;
using legallead.json.interfaces;

namespace legallead.json.db.entity
{
    public abstract class DataEntity<T> : BaseEntity<T>, IDataEntity
        where T : class, IDataEntity, new()
    {
    }
}
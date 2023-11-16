using legallead.json.entities;
using legallead.json.interfaces;

namespace legallead.json.tests
{
    public class UserEntity : BaseEntity<UserEntity>, IDataEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Pwd { get; set; } = string.Empty;

    }
}

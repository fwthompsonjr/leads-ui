using legallead.permissions.api.Attr;

namespace legallead.models
{
    public class UserLevelRequest
    {
        [LevelRequest]
        public string Level { get; set; } = string.Empty;
    }
}
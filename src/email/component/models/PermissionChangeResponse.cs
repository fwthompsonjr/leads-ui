using Newtonsoft.Json;

namespace legallead.email.models
{
    public class PermissionChangeResponse
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public PermissionLevelResponseBo Dto { get; set; } = new();

        [JsonIgnore]
        public PermissionDiscountRequest? DiscountRequest
        {
            get
            {
                return _discountRequest;
            }
            internal set
            {
                _discountRequest = value;
            }
        }
        [JsonIgnore]
        public PermissionLevelRequest? LevelRequest
        {
            get
            {
                return _levelRequest;
            }
            internal set
            {
                _levelRequest = value;
            }
        }

        private PermissionDiscountRequest? _discountRequest;
        private PermissionLevelRequest? _levelRequest;
    }
}

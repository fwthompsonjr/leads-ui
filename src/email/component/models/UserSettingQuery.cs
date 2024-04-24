using Dapper;

namespace legallead.email.models
{
    internal class UserSettingQuery
    {
        public string? Id { get; set; }
        public string? Email { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Id) || !string.IsNullOrWhiteSpace(Email);
            }
        }

        public DynamicParameters GetParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("user_index", Id);
            parameters.Add("email_address", Email);
            return parameters;
        }
    }
}

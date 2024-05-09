using Dapper;

namespace legallead.email.models
{
    public class UserAccountByUserNameBo
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName);
            }
        }

        public DynamicParameters GetParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("user_name", UserName);
            return parameters;
        }
    }
}
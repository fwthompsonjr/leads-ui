using Dapper;

namespace legallead.email.models
{
    public class UserAccountByEmailBo
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Email);
            }
        }

        public DynamicParameters GetParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("email_address", Email);
            return parameters;
        }

        public DynamicParameters GetSearchParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("search_index", Id);
            return parameters;
        }
    }
}
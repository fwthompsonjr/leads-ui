using Dapper;
using legallead.email.entities;

namespace legallead.email.models
{
    internal static class LogCorrespondenceRequest
    {

        public static DynamicParameters GetParameters(LogCorrespondenceDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("user_index", dto.Id);
            parameters.Add("json_data", dto.JsonData);
            return parameters;
        }
        public static DynamicParameters GetParameters(LogCorrespondenceSuccessDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("user_index", dto.Id);
            return parameters;
        }
    }
}

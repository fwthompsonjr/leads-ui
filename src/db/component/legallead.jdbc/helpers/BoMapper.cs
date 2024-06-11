using Dapper;
using legallead.jdbc.entities;
using Newtonsoft.Json;

namespace legallead.jdbc.helpers
{
    internal static class BoMapper
    {
        public static T? MapFrom<K, T>(K? source)
        {
            try
            {
                if (object.Equals(source, default(K))) return default;
                var json = JsonConvert.SerializeObject(source);
                var response = JsonConvert.DeserializeObject<T>(json);
                return response;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static EmailBodyBo? Map(EmailBodyDto? source)
        {
            return MapFrom<EmailBodyDto, EmailBodyBo>(source);
        }

        public static EmailCountBo? Map(EmailCountDto? source)
        {
            return MapFrom<EmailCountDto, EmailCountBo>(source);
        }
        public static List<EmailListBo>? Map(IEnumerable<EmailListDto>? source)
        {
            return MapFrom<IEnumerable<EmailListDto>, List<EmailListBo>>(source);
        }

        public static DynamicParameters GetCountParameters(string? userId)
        {
            var parms = new DynamicParameters();
            parms.Add("user_index", userId);
            return parms;
        }
        public static DynamicParameters GetBodyParameters(string? messageId, string? userId)
        {
            var parms = new DynamicParameters();
            parms.Add("user_index", userId);
            parms.Add("message_id", messageId);
            return parms;
        }
        public static DynamicParameters GetMessagesParameters(string? userId, DateTime? lastUpdate)
        {
            var parms = new DynamicParameters();
            parms.Add("user_index", userId);
            parms.Add("last_created_date", lastUpdate);
            return parms;
        }

    }
}

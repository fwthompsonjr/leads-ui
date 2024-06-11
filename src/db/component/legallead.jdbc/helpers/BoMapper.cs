using Dapper;
using legallead.jdbc.entities;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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

        public static string? FromBase64(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (!value.IsBase64()) return value;
            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }
        [ExcludeFromCodeCoverage(Justification = "Private method tested from public member.")]
        private static bool IsBase64(this string base64String)
        {
            // Credit: oybek https://stackoverflow.com/users/794764/oybek
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
               || base64String.Contains(' ') || base64String.Contains('\t') || base64String.Contains('\r') || base64String.Contains('\n'))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}

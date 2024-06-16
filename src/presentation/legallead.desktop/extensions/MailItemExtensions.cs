using legallead.desktop.models;
using System;

namespace legallead.desktop.extensions
{
    internal static class MailItemExtensions
    {
        public static MailStorageItem ToStorage(this MailItem source)
        {
            var createDate = source.CreateDate.HasValue ?
                source.CreateDate.Value.ToLocalDateTime().ToString("f").ToDdd() : " - ";
            return new()
            {
                Id = source.Id,
                UserId = source.UserId,
                FromAddress = source.FromAddress,
                ToAddress = source.ToAddress,
                Subject = source.Subject,
                CreateDate = createDate
            };
        }

        private static DateTime ToLocalDateTime(this DateTime source)
        {
            DateTime convertedDate = DateTime.SpecifyKind(
            source,
            DateTimeKind.Utc);

            return convertedDate.ToLocalTime();
        }

        private static string ToDdd(this string date)
        {
            const char comma = ',';
            if (string.IsNullOrEmpty(date) || !date.Contains(comma)) { return date; }
            var components = date.Split(comma);
            components[0] = components[0].Substring(0, 3);
            return string.Join(comma, components);
        }
    }
}

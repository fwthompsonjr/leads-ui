using legallead.desktop.entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace legallead.desktop.utilities
{
    internal static class BlankSubmissionHandler
    {
        public static object SubstituteBlankValues(Type objectType, object? value)
        {
            if (value == null) return new();
            _ = BlankItemMap.TryGetValue(objectType, out var item);
            if (item == null) return value;
            if (value is ContactName[] names) return Substitute(names, item);
            if (value is ContactAddress[] addresses) return Substitute(addresses, item);
            if (value is ContactPhone[] phones) return Substitute(phones, item);
            if (value is ContactEmail[] emails) return Substitute(emails, item);
            return value;
        }

        private static object Substitute(ContactName[] names, string item)
        {
            var list = names.ToList();
            list.ForEach(n =>
            {
                if (string.IsNullOrWhiteSpace(n.Name)) n.Name = item;
            });
            return list;
        }

        private static object Substitute(ContactAddress[] addresses, string item)
        {
            var list = addresses.ToList();
            list.ForEach(n =>
            {
                if (string.IsNullOrWhiteSpace(n.Address)) n.Address = item;
            });
            return list;
        }
        private static object Substitute(ContactPhone[] phones, string item)
        {
            var list = phones.ToList();
            list.ForEach(n =>
            {
                if (string.IsNullOrWhiteSpace(n.Phone)) n.Phone = item;
            });
            return list;
        }

        private static object Substitute(ContactEmail[] emails, string item)
        {
            var list = emails.ToList();
            list.ForEach(n =>
            {
                if (string.IsNullOrWhiteSpace(n.Email)) n.Email = item;
            });
            return list;
        }

        private static readonly Dictionary<Type, string> BlankItemMap = new()
         {
             { typeof(ContactName[]), "(left blank by user)"  },
             { typeof(ContactAddress[]),  "123 Blank Address, Nowhere, NO 00000"},
             { typeof(ContactPhone[]),  "012-345-6789"},
             { typeof(ContactEmail[]),  "left.blank.by.user@nonentry.com" }
         };
    }
}

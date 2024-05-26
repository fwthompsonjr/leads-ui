namespace legallead.permissions.api.Utility
{
    internal static class BlankProfileHandler
    {
        public static object SubstituteBlankValues(Type objectType, object? value)
        {
            if (value == null) return new();
            _ = BlankItemMap.TryGetValue(objectType, out var item);
            if (item == null) return value;
            if (value is ChangeContactNameRequest[] names) return Substitute(names, item);
            if (value is ChangeContactAddressRequest[] addresses) return Substitute(addresses, item);
            if (value is ChangeContactPhoneRequest[] phones) return Substitute(phones, item);
            if (value is ChangeContactEmailRequest[] emails) return Substitute(emails, item);
            return value;
        }

        private static ChangeContactNameRequest[] Substitute(ChangeContactNameRequest[] names, string item)
        {
            var list = names.ToList();
            list.ForEach(n =>
            {
                if (string.IsNullOrEmpty(n.Name)) n.Name = string.Empty;
                if (n.Name.Equals(item)) n.Name = string.Empty;
            });
            return [.. list];
        }

        private static ChangeContactAddressRequest[] Substitute(ChangeContactAddressRequest[] addresses, string item)
        {
            var list = addresses.ToList();
            list.ForEach(n =>
            {
                if (string.IsNullOrEmpty(n.Address)) n.Address = string.Empty;
                if (n.Address.Equals(item)) n.Address = string.Empty;
            });
            return [.. list];
        }
        private static ChangeContactPhoneRequest[] Substitute(ChangeContactPhoneRequest[] phones, string item)
        {
            var list = phones.ToList();
            list.ForEach(n =>
            {
                if (string.IsNullOrEmpty(n.Phone)) n.Phone = string.Empty;
                if (n.Phone.Equals(item)) n.Phone = string.Empty;
            });
            return [.. list];
        }

        private static ChangeContactEmailRequest[] Substitute(ChangeContactEmailRequest[] emails, string item)
        {
            var list = emails.ToList();
            list.ForEach(n =>
            {
                if (string.IsNullOrEmpty(n.Email)) n.Email = string.Empty;
                if (n.Email.Equals(item)) n.Email = string.Empty;
            });
            return [.. list];
        }

        private static readonly Dictionary<Type, string> BlankItemMap = new()
         {
             { typeof(ChangeContactNameRequest[]), "(left blank by user)"  },
             { typeof(ChangeContactAddressRequest[]),  "123 Blank Address, Nowhere, NO 00000"},
             { typeof(ChangeContactPhoneRequest[]),  "012-345-6789"},
             { typeof(ChangeContactEmailRequest[]),  "left.blank.by.user@nonentry.com" }
         };
    }
}

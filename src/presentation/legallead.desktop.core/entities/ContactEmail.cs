using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.entities
{
    internal class ContactEmail
    {
        public string EmailType { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public ContactProfileItem ToItem()
        {
            return new ContactProfileItem
            {
                Category = GetType().Name.Replace("Contact", ""),
                Code = EmailType,
                Data = Email,
            };
        }
    }
}
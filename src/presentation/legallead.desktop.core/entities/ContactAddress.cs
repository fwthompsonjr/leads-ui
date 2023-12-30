using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace legallead.desktop.entities
{
    internal class ContactAddress
    {
        public string AddressType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public ContactProfileItem ToItem()
        {
            return new ContactProfileItem
            {
                Category = GetType().Name.Replace("Contact", ""),
                Code = AddressType,
                Data = Address,
            };
        }
    }
}
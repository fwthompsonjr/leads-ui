using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.entities
{
    internal class UserBo
    {
        public bool IsAuthenicated { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ApiContext[]? Applications { get; set; }
        public bool IsInitialized => Applications != null;
    }
}
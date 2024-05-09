using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.email.models
{
    public class PasswordChangedResponse(string userName)
    {
        public string UserName { get; set; } = userName;
    }
}

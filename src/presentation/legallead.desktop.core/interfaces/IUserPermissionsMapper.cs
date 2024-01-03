using legallead.desktop.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.interfaces
{
    internal interface IUserPermissionsMapper
    {
        Task<string> Map(IPermissionApi api, UserBo user, string source);
    }
}
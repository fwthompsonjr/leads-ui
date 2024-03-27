using legallead.installer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.installer.Interfaces
{
    public interface IGitReader
    {
        Task<List<ReleaseModel>?> GetReleases();
    }
}

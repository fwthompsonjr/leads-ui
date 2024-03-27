using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.installer.Models
{
    internal class ReleaseModel
    {
        public long Id { get; set; }
        public long RepositoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

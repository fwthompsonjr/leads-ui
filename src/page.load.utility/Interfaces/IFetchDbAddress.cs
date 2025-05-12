using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using page.load.utility.Entities;

namespace page.load.utility.Interfaces
{
    public interface IFetchDbAddress
    {
        public TheAddress FindAddress(int countyId, string caseNumber);
    }
}

using page.load.utility.Entities;

namespace page.load.utility.Interfaces
{
    public interface IFetchDbAddress
    {
        public TheAddress? FindAddress(int countyId, string caseNumber);
    }
}

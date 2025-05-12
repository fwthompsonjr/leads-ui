using legallead.permissions.api.Extensions;
using page.load.utility.Entities;
using page.load.utility.Interfaces;

namespace legallead.permissions.api.Services
{
    public class CaseDetailLookupService(IUserUsageService usageDb) : IFetchDbAddress
    {
        private readonly IUserUsageService db = usageDb;

        public TheAddress? FindAddress(int countyId, string caseNumber)
        {
            var db_item = db.FindCaseItemByCaseNumberAsync(countyId, caseNumber)
                .GetAwaiter()
                .GetResult();
            if (string.IsNullOrEmpty(db_item)) return default;
            var address = db_item.ToInstance<TheAddress>();
            if (address == null || !address.IsValid || address.Zip.Equals("00000")) return default;
            return address;
        }
    }
}

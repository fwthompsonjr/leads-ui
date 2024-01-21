using legallead.jdbc.interfaces;
using legallead.records.search.Models;
using OfficeOpenXml;

namespace legallead.search.api.Services
{
    internal class ExcelGenerator : IExcelGenerator
    {
        public ExcelPackage? GetAddresses(WebFetchResult fetchResult, ILoggingRepository logging)
        {
            throw new NotImplementedException();
        }

        public bool SerializeResult(string uniqueId, ExcelPackage package, ISearchQueueRepository repo, ILoggingRepository logging)
        {
            try
            {
                using var ms = new MemoryStream();
                package.SaveAs(ms);
                var content = ms.ToArray();
                _ = repo.Content(uniqueId, content).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                _ = logging.LogError(ex).ConfigureAwait(false);
                return false;
            }
        }
    }
}

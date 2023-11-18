using System.Data;
using System.Globalization;

namespace legallead.harriscriminal.db
{
    public static class CaseStyleDbInspector
    {
        private static CultureInfo GetCulture => CultureInfo.InvariantCulture;
        private static StringComparison Oic => StringComparison.OrdinalIgnoreCase;

        public static bool HasHeader(DateTime filingDate)
        {
            var db = Startup.CaseStyles.DataList;
            var fileDate = filingDate.ToString("M/d/yyyy", GetCulture);
            return db.Exists(f => f.FileDate.Equals(fileDate, Oic));
        }

        public static bool HasDetail(DateTime filingDate)
        {
            if (!HasHeader(filingDate))
            {
                return false;
            }
            var db = Startup.Downloads.DataList;
            var fileDate = filingDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var datas = db.SelectMany(f => f.Data);
            return datas.Any(a => a.FilingDate.Equals(fileDate, Oic));
        }

        public static bool HasDetail(DateTime filingDate, string caseNumber)
        {
            if (!HasDetail(filingDate))
            {
                return false;
            }
            var db = Startup.Downloads.DataList;
            var fileDate = filingDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            var datas = db.SelectMany(f => f.Data);
            return datas.Any(a => a.FilingDate.Equals(fileDate, Oic) && a.CaseNumber.Equals(caseNumber, Oic));
        }
    }
}
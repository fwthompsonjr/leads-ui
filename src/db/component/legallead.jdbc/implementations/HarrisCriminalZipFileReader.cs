using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;
using System.IO.Compression;

namespace legallead.jdbc.implementations
{
    public class HarrisCriminalZipFileReader(
        string zipname,
        IHarrisLoadRepository repo) : IDisposable
    {
        private const int MxRecords = 500;
        protected readonly string zipFileName = zipname;
        private readonly IHarrisLoadRepository _db = repo;
        protected readonly string tempFileName = Path.ChangeExtension(Path.GetRandomFileName(), Guid.NewGuid().ToString() + ".tr5");
        private List<List<string>>? rawData = null;
        private bool disposedValue;

        public virtual void Read()
        {
            if (string.IsNullOrEmpty(zipFileName) || !File.Exists(zipFileName))
                throw new InvalidOperationException();
            using var zip = ZipFile.OpenRead(zipFileName);
            var entry = zip.Entries.FirstOrDefault();
            if (entry == null) return;
            entry.ExtractToFile(tempFileName);
        }
        public void Translate()
        {
            Read();
            rawData = [];
            if (!File.Exists(tempFileName)) return;
            using var reader = new StreamReader(tempFileName);
            while ((!reader.EndOfStream))
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var data = line.Split("\t").ToList();
                rawData.Add(data);
            }
        }

        public void Transfer()
        {
            if (rawData == null) Translate();
            if (rawData == null) return;
            if (rawData.Count <= 1) return;
            var header = rawData[0];
            var records = new List<HarrisCriminalRecordDto>();
            var lastidnx = rawData.Count - 1;
            for (var r = lastidnx; r >= 1; r--)
            {
                var datum = new HarrisCriminalRecordDto();
                var data = rawData[r];
                for (var c = 0; c < header.Count; c++)
                {
                    var fld = header[c];
                    var item = data[c];
                    var mapped = HarrisLookupService.Translate(fld, item);
                    datum[c] = mapped;
                }
                records.Add(datum);
                if (records.Count == MxRecords)
                {
                    Post(records);
                    records.Clear();
                }
            }
            if (records.Count > 0) { Post(records); }
        }

        protected void Post(List<HarrisCriminalRecordDto> records)
        {
            var js = JsonConvert.SerializeObject(records, Formatting.Indented);
            _ = _db.Append(js).GetAwaiter().GetResult();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
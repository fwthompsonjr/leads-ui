using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;
using System.IO.Compression;

namespace legallead.jdbc.implementations
{
    public class HarrisCriminalZipFileReader : IDisposable
    {
        public HarrisCriminalZipFileReader(string zipname,
        IHarrisLoadRepository repo,
        bool allowFileOperations = true)
        {
            zipFileName = zipname;
            _db = repo;
            useFileSystem = allowFileOperations;
            if (allowFileOperations)
            {
                tempFileName = Path.ChangeExtension(Path.GetRandomFileName(), Guid.NewGuid().ToString() + ".tr5");
            } else
            {
                tempFileName = string.Empty;
            }
        }
        private const int MxRecords = 500;
        protected readonly string zipFileName;
        private readonly IHarrisLoadRepository _db;
        protected readonly string tempFileName; 
        protected readonly bool useFileSystem;
        protected string decodedData = string.Empty;
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
            if (!useFileSystem)
            {
                if (string.IsNullOrEmpty(decodedData)) return;
                var items = decodedData.Split(Environment.NewLine).ToList();
                items.ForEach(item =>
                {
                    var data = item.Split("\t").ToList();
                    rawData.Add(data);
                });
                return;
            }
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
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
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
                    // fetch field number
                    var fieldId = HarrisCriminalFieldName.Fields.FindIndex(x => x.Key.Equals(fld, oic));
                    if (fieldId < 0) { fieldId = c; }
                    var mapped = HarrisLookupService.Translate(fld, item);
                    datum[fieldId] = mapped;
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
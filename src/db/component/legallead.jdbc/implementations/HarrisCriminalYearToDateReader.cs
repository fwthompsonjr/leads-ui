using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.implementations
{
    public class HarrisCriminalYearToDateReader : IDisposable
    {
        private readonly string zipFileName;
        private readonly string tempFileName;
        private List<List<string>>? rawData = null;
        private string zipContent = string.Empty;
        private bool disposedValue;

        public HarrisCriminalYearToDateReader(string zipname)
        {
            zipFileName = zipname;
            tempFileName = Path.ChangeExtension(Path.GetRandomFileName(), Guid.NewGuid().ToString() + ".tr5");
        }

        public void Read()
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
            for ( var r = 1; r < rawData.Count; r++)
            {
                var data = rawData[r];
                for (var c = 0; c < header.Count; c++) 
                { 
                    var fld = header[c];
                    var item = data[c];
                    // var mapped = HarrisLookupService.
                }
            }
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
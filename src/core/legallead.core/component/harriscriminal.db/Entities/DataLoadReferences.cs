using legallead.harriscriminal.db.Interfaces;
using legallead.harriscriminal.db.Tables;

namespace legallead.harriscriminal.db.Entities
{
    public class DataLoadReferences : BaseDataLoad, IDataLoad<ReferenceTable>
    {
        public override List<string> FileNames { get; set; } = new();

        public List<ReferenceTable> Load(bool reset = false)
        {
            var progressHandler = new Progress<DataLoadDto>(dto =>
            {
                if (dto.IsComplete)
                {
                    Console.WriteLine("Task has completed");
                }
            });
            var progress = progressHandler as IProgress<DataLoadDto>;
            return LoadAsyc(progress).Result;
        }

        public async Task<List<ReferenceTable>> LoadAsyc(IProgress<DataLoadDto> progress, bool reset = false)
        {
            // build list of files
            Read(reset);
            var dto = new DataLoadDto
            {
                Count = FileNames.Count,
                StartTime = DateTime.Now
            };

            var tables = new List<ReferenceTable>();
            await Task.Run(() =>
            {
                Fetch(progress, dto, tables);
            });
            return tables;
        }

        /// <summary>
        /// Reads Reference Data and Stores in Memory
        /// </summary>
        /// <param name="reset"></param>
        private void Read(bool reset = false)
        {
            if (!reset && FileNames != null && FileNames.Count > 0)
            {
                return;
            }
            const string extn = "*hcc.tables.*.json";
            var directory = new DirectoryInfo(DataFolder);
            var files = directory.GetFiles(extn).ToList();
            FileNames = files.Select(f => f.FullName).ToList();
        }
    }
}
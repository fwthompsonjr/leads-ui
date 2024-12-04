using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class HarrisCriminalFileReader(
        string zipname,
        IHarrisLoadRepository repo) : HarrisCriminalZipFileReader(zipname, repo)
    {

        public override void Read()
        {
            if (string.IsNullOrEmpty(zipFileName) || !File.Exists(zipFileName))
                throw new InvalidOperationException();
            var content = File.ReadAllText(zipFileName);
            if (string.IsNullOrEmpty(content)) return;
            if (File.Exists(tempFileName)) File.Delete(tempFileName);
            File.WriteAllText(tempFileName, content);
        }
    }
}

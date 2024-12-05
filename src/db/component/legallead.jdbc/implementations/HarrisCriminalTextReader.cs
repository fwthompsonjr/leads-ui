using legallead.jdbc.interfaces;
using System.Text;

namespace legallead.jdbc.implementations
{
    public class HarrisCriminalTextReader(
        string zipname,
        IHarrisLoadRepository repo) : HarrisCriminalZipFileReader(zipname, repo, false)
    {
         

        public override void Read()
        {
            if (string.IsNullOrEmpty(zipFileName)) throw new InvalidOperationException();
            var content = DecodeContent();
            if (string.IsNullOrEmpty(content)) return;
            decodedData = content;
        }

        private string DecodeContent()
        {
            try
            {
                var converted = Convert.FromBase64String(zipFileName);
                return Encoding.UTF8.GetString(converted);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}

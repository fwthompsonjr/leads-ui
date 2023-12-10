namespace legallead.permissions.api.Utility
{
    public class PostpreCertificateStartUpTask : IStartupTask
    {
        public int Index => 0;

        public async Task Execute()
        {
            try
            {
                var path = GetHomePath();
                if (string.IsNullOrEmpty(path)) return;
                var targetDir = Path.Combine(path, "postgresql");
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                var targetFile = Path.Combine(targetDir, "root.crt");
                if (File.Exists(targetFile)) return;
                var content = await GetContent();
                File.WriteAllText(targetFile, content);
            }
            catch
            {
                Console.WriteLine("Failed to install root certificate.");
            }
        }


        private async static Task<string> GetContent()
        {
            const string address = "https://cockroachlabs.cloud/clusters/990093b4-3650-4572-a86f-02439ba2f071/cert";
            using HttpClient client = new();
            HttpRequestMessage request = new(HttpMethod.Get, address);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        private static string? GetHomePath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return Environment.GetEnvironmentVariable("HOME");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
    }
}
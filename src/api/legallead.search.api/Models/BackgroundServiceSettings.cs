namespace legallead.search.api.Models
{
    public class BackgroundServiceSettings : IBackgroundServiceSettings
    {
        public bool Enabled { get; set; } = true;
        public int Delay { get; set; } = 45;
        public int Interval { get; set; } = 3;
    }
}

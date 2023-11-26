namespace legallead.harriscriminal.db.Entities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S6561:Avoid using \"DateTime.Now\" for benchmarking or timing operations",
            Justification = "Will refactor at later date")]
    public class DataLoadDto
    {
        private TimeSpan _elapsed;

        private bool _isComplete;
        public int Count { get; set; }
        public int Processed { get; set; }
        public DateTime StartTime { get; set; }

        public TimeSpan Elapsed
        {
            get
            {
                if (IsComplete) return _elapsed;
                _elapsed = DateTime.Now.Subtract(StartTime);
                return _elapsed;
            }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            set
            {
                if (value)
                {
                    _elapsed = DateTime.Now.Subtract(StartTime);
                }
                _isComplete = value;
            }
        }
    }
}
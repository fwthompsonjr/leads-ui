using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Classes
{
    //This is a new class that represents a progress object.
    public class ZipProgress
    {
        public ZipProgress(int total, int processed, string currentItem)
        {
            Total = total;
            Processed = processed;
            CurrentItem = currentItem;
        }
        public int Total { get; }
        public int Processed { get; }
        public string CurrentItem { get; }

        [ExcludeFromCodeCoverage(Justification = "Item is tested in integration testing.")]
        public void Echo()
        {
            try
            {
                var min = 0.00000000001d;
                var percentage = Math.Round(Convert.ToDouble(Processed) / (min + Convert.ToDouble(Total)), 4) * 100d;
                var percentageId = Convert.ToInt32(Math.Floor(percentage));
                if (percentageId % 5 == 0)
                {
                    var message = $"Processed {Processed:D3} of {Total:D3} : {percentage:F2}% completed.";
                    Console.WriteLine(message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("... Download in progress");
            }
        }
    }
}

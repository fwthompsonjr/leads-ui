﻿namespace legallead.records.search.Dto
{
    public class HarrisCaseDateDto
    {
        public const string DateFormat = "MM/dd/yyyy";
        public TimeSpan Interval { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate => StartDate.Add(Interval);

        public string StartingDate => StartDate.ToString(DateFormat, CultureInfo.InvariantCulture);

        public string EndingDate => EndDate.ToString(DateFormat, CultureInfo.InvariantCulture);

        /// <summary>
        /// Builds the list.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="rangeInDays">The range in days.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// interval
        /// or
        /// rangeInDays
        /// </exception>
        public static List<HarrisCaseDateDto> BuildList(
            DateTime startDate,
            TimeSpan interval,
            int rangeInDays)
        {
            if (Math.Abs(interval.TotalDays) < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(interval));
            }
            if (rangeInDays == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rangeInDays));
            }
            rangeInDays = Math.Abs(rangeInDays);
            HarrisCaseDateDto obj = new()
            {
                StartDate = startDate,
                Interval = interval
            };
            List<HarrisCaseDateDto> list = new() { obj };
            DateTime top = obj.StartDate;
            TimeSpan compared;
            try
            {
                do
                {
                    HarrisCaseDateDto item = list[^1];
                    HarrisCaseDateDto dto = new() { Interval = obj.Interval, StartDate = item.EndDate };
                    list.Add(dto);
                    DateTime bottom = list[^1].EndDate;
                    compared = top.Subtract(bottom);
                } while (Math.Abs(compared.TotalDays) < rangeInDays);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return list;
        }
    }
}
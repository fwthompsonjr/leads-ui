﻿namespace legallead.records.search.Dto
{
    public static class HarrisCriminalExtensions
    {
        public static void Append(this List<HarrisCriminalStyleDto> list, IEnumerable<HarrisCriminalStyleDto> source)
        {
            if (list == null)
            {
                return;
            }

            if (source == null)
            {
                return;
            }

            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            foreach (HarrisCriminalStyleDto item in source)
            {
                HarrisCriminalStyleDto? found = list.Find(f => f.CaseNumber.Equals(item.CaseNumber, oic) &
                    f.Court.Equals(item.Court, oic) &
                    f.FileDate.Equals(item.FileDate, oic));
                if (found == null) { list.Add(item); }
            }
        }
    }
}
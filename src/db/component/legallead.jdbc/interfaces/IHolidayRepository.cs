using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IHolidayRepository
    {
        Task<bool> IsHolidayAsync(DateTime date);
        Task<List<HoliDateBo>?> GetHolidaysAsync();
    }
}

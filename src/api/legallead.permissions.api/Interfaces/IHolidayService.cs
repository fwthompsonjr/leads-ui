using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IHolidayService
    {
        Task<bool> IsHolidayAsync(string holidayDate);
        Task<List<HolidayResponse>?> GetHolidaysAsync();
    }
}

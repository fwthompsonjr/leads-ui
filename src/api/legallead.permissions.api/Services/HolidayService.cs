using AutoMapper;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;
using System.Globalization;

namespace legallead.permissions.api.Services
{
    public class HolidayService(IHolidayRepository db) : IHolidayService
    {
        private readonly IHolidayRepository _db = db;
        public async Task<List<HolidayResponse>?> GetHolidaysAsync()
        {
            var response = await _db.GetHolidaysAsync();
            if (response == null) return [];
            var list = new List<HolidayResponse>();
            response.ForEach(r => list.Add(_mapper.Map<HolidayResponse>(r)));
            return list;
        }

        public async Task<bool> IsHolidayAsync(string holidayDate)
        {
            if (string.IsNullOrWhiteSpace(holidayDate)) return false;
            if (!DateTime.TryParse(holidayDate,
                _culture.DateTimeFormat,
                DateTimeStyles.AssumeLocal,
                out var dte)) return false;
            var response = await _db.IsHolidayAsync(dte);
            return response;
        }

        private static readonly IMapper _mapper = ModelMapper.Mapper;
        private readonly static CultureInfo _culture = new("en-us");
    }
}

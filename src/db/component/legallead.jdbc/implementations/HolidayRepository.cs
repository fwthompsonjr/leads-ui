using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class HolidayRepository(DataContext context) :
        BaseRepository<HoliDateDto>(context), IHolidayRepository
    {
        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            var prc = ProcedureNames.FindProc;
            var dateString = date.ToString("yyyy-MM-dd");
            var parameters = new DynamicParameters();
            parameters.Add("holiday_date", dateString);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<HoliDateDto>(connection, prc, parameters);
                return response != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<HoliDateBo>?> GetHolidaysAsync()
        {
            var prc = ProcedureNames.QueryProc;
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<HoliDateDto>(connection, prc);
                if (response == null) return default;
                var list = new List<HoliDateBo>();
                response.ToList().ForEach(r =>
                {
                    list.Add(MapFrom(r));
                });
                return list;
            }
            catch (Exception)
            {
                return default;
            }
        }

        private static HoliDateBo MapFrom(HoliDateDto response)
        {
            return new()
            {
                Id = response.Id,
                HoliDate = response.HoliDate,
                CreateDate = response.CreateDate
            };
        }
        private static class ProcedureNames
        {
            public const string FindProc = "CALL USP_QUERY_IS_HOLIDAY( ? );";
            public const string QueryProc = "CALL USP_QUERY_GET_HOLIDAYS();";
        }
    }
}
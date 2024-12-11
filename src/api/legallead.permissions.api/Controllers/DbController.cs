using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace legallead.permissions.api.Controllers
{
    [Route("/db")]
    [ApiController]
    public class DbController(
    ILeadAuthenicationService lead,
    IDbHistoryService db,
    IHolidayService holidayDb) : ControllerBase
    {
        private readonly ILeadAuthenicationService _leadService = lead;
        private readonly IDbHistoryService _dataService = db;
        private readonly IHolidayService _holidayService = holidayDb;
        [HttpPost("begin")]
        public async Task<IActionResult> BeginAsync(BeginDataRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                var response = await _dataService.BeginAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteAsync(CompleteDataRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                var response = await _dataService.CompleteAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        [HttpPost("find")]
        public async Task<IActionResult> FindAsync(FindDataRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                var response = await _dataService.FindAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync(UploadDataRequest model)
        {
            try
            {
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();
                var response = await _dataService.UploadAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }



        [HttpPost("is-holiday")]
        public async Task<IActionResult> IsHolidayAsync(QueryHolidayRequest model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.HolidayDate)) return BadRequest();
                if (!DateTime.TryParse(model.HolidayDate,
                    _culture.DateTimeFormat,
                    DateTimeStyles.AssumeLocal,
                    out var dte)) return BadRequest($"Invalid date input: {model.HolidayDate}");

                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();

                var response = await _holidayService.IsHolidayAsync(model.HolidayDate);
                var data = new IsHolidayResponse
                {
                    IsHoliday = response,
                    HolidayDate = dte
                };
                return Ok(data);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }


        [HttpPost("get-holiday-list")]
        public async Task<IActionResult> QueryHolidayAsync(QueryHolidayRequest model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.HolidayDate)) return BadRequest();
                var user = _leadService.GetUserModel(Request, UserAccountAccess);
                if (user == null) return Unauthorized();

                var response = await _holidayService.GetHolidaysAsync();
                return Ok(response ?? []);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        private const string UserAccountAccess = "user account access credential";
        private readonly static CultureInfo _culture = new("en-us");

    }
}
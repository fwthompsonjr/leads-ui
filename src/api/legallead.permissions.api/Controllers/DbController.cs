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
    IHolidayService holidayDb,
    IUserUsageService usageDb) : ControllerBase
    {
        private readonly ILeadAuthenicationService _leadService = lead;
        private readonly IDbHistoryService _dataService = db;
        private readonly IHolidayService _holidayService = holidayDb;
        private readonly IUserUsageService _usageService = usageDb;

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

        [HttpPost("usage-append-record")]
        public async Task<IActionResult> UsageAppendAsync(AppendUsageRecordRequest request)
        {
            if (string.IsNullOrEmpty(request.LeadUserId)) return BadRequest();
            if (request.CountyId == 0) return BadRequest();
            var respone = (await _usageService.AppendUsageRecordAsync(request)) ?? new();
            return Ok(respone);
        }

        [HttpPost("usage-complete-record")]
        public async Task<IActionResult> UsageCompleteAsync(CompleteUsageRecordRequest request)
        {
            if (string.IsNullOrEmpty(request.UsageRecordId)) return BadRequest();
            if (request.RecordCount == 0) return BadRequest();
            var response = (await _usageService.CompleteUsageRecordAsync(request)) ?? new();
            return Ok(response);
        }


        [HttpPost("usage-get-monthly-limit")]
        public async Task<IActionResult> UsageGetLimitAsync(GetMonthlyLimitRequest request)
        {
            if (string.IsNullOrEmpty(request.LeadId)) return BadRequest();
            var response = (await _usageService.GetMonthlyLimitAsync(request)) ?? new();
            return Ok(response);
        }

        [HttpPost("usage-get-history")]
        public async Task<IActionResult> UsageGetHistoryAsync(GetUsageRequest request)
        {
            if (string.IsNullOrEmpty(request.LeadId)) return BadRequest();
            var response = (await _usageService.GetUsageAsync(request)) ?? new();
            return Ok(response);
        }

        [HttpPost("usage-get-summary")]
        public async Task<IActionResult> UsageGetSummaryAsync(GetUsageRequest request)
        {
            if (string.IsNullOrEmpty(request.LeadId)) return BadRequest();
            var response = (await _usageService.GetUsageSummaryAsync(request)) ?? new();
            return Ok(response);
        }

        [HttpPost("usage-set-monthly-limit")]
        public async Task<IActionResult> UsageSetLimitAsync(SetMonthlyLimitRequest request)
        {
            if (string.IsNullOrEmpty(request.LeadId)) return BadRequest();
            var response = (await _usageService.SetMonthlyLimitAsync(request)) ?? new();
            return Ok(response);
        }

        private const string UserAccountAccess = "user account access credential";
        private readonly static CultureInfo _culture = new("en-us");

    }
}
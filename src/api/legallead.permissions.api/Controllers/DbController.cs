using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;
using page.load.utility;
using page.load.utility.Entities;
using page.load.utility.Extensions;
using System.Collections.ObjectModel;
using System.Globalization;

namespace legallead.permissions.api.Controllers
{
    [Route("/db")]
    [ApiController]
    public class DbController(
    ILeadAuthenicationService lead,
    IDbHistoryService db,
    IHolidayService holidayDb,
    IUserUsageService usageDb,
    ICountyFileService fileDb) : ControllerBase
    {
        private readonly ILeadAuthenicationService _leadService = lead;
        private readonly IDbHistoryService _dataService = db;
        private readonly IHolidayService _holidayService = holidayDb;
        private readonly IUserUsageService _usageService = usageDb;
        private readonly ICountyFileService _fileService = fileDb;

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

        [HttpPost("usage-get-excel-file-detail")]
        public async Task<IActionResult> GetExcelDetailByIdAsync(CompleteUsageRecordRequest request)
        {
            if (string.IsNullOrEmpty(request.UsageRecordId)) return BadRequest();
            var response = await _usageService.GetExcelDetailAsync(request.UsageRecordId);
            return Ok(response);
        }

        [HttpPost("usage-set-monthly-limit")]
        public async Task<IActionResult> UsageSetLimitAsync(SetMonthlyLimitRequest request)
        {
            if (string.IsNullOrEmpty(request.LeadId)) return BadRequest();
            var response = (await _usageService.SetMonthlyLimitAsync(request)) ?? new();
            return Ok(response);
        }

        [HttpPost("save-search-file")]
        public async Task<IActionResult> SaveContentAsync(DbCountyFileModel request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var response = await _fileService.SaveAsync(request);
            return Ok(response);
        }

        [HttpPost("get-search-file")]
        public async Task<IActionResult> GetContentAsync(DbCountyFileModel request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var response = await _fileService.GetAsync(request);
            return Ok(response);
        }

        [HttpPost("process-offline")]
        public async Task<IActionResult> ProcessOfflineAsync(BulkReadRequest request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var response = new BulkReadResponse { RequestId = request.RequestId };
            var settings = request.Cookies.ToInstance<List<CookieModel>>();
            var items = request.Workload.ToInstance<List<CaseItemDto>>();
            if (settings == null || items == null) return Ok(response);
            var model = new OfflineDataModel
            {
                RequestId = request.RequestId,
                Cookie = request.Cookies,
                Workload = request.Workload,
                RowCount = items.Count
            };
            model = await _usageService.AppendOfflineRecordAsync(model);
            var cookies = settings.Select(s => s.Cookie()).ToList();
            response.IsValid = true;
            var objRequestGuid =
                string.IsNullOrEmpty(model.OfflineId) ?
                Guid.NewGuid().ToString("D") : model.OfflineId;
            response.OfflineRequestId = objRequestGuid;
            offlineRequests.Add(response);
            // need to create an instance of new interface that fetches address from db
            // this allows the reader to get from db in place of http call when data
            // has already been read
            var service = new BulkCaseReader(
                new ReadOnlyCollection<OpenQA.Selenium.Cookie>(cookies),
                items,
                new BulkReadMessages { OfflineRequestId = objRequestGuid })
            {
                OnStatusUpdated = StatusChanged,
                OnStatusTimeOut = StatusTerminated
            };
            _ = Task.Run(() =>
            {
                var rsp = service.Execute();
                if (rsp is not string json) return;
                var find = offlineRequests.FirstOrDefault(x => x.OfflineRequestId == response.OfflineRequestId);
                if (find != null)
                {
                    find.IsCompleted = true;
                    find.Content = json;
                }
            });
            return Ok(response);
        }

        [HttpPost("get-offline-requests")]
        public async Task<IActionResult> GetOfflineStatusAsync(UserOfflineStatusRequest request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            if (!Guid.TryParse(request.LeadId, out var _)) return BadRequest("Invalid Lead Id");
            var data = await _usageService.GetOfflineStatusAsync(request.LeadId);
            return Ok(data);
        }
        [HttpPost("get-offline-request-search-details")]
        public async Task<IActionResult> GetOfflineStatusDetailAsync(UserOfflineStatusRequest request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            if (!Guid.TryParse(request.LeadId, out var _)) return BadRequest("Invalid Lead Id");
            var data = await _usageService.GetOfflineSearchTypesByIdAsync(request.LeadId);
            return Ok(data);
        }
        [HttpPost("process-offline-set-context")]
        public async Task<IActionResult> ProcessOfflineSetContextAsync(BulkReadRequest request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var model = new OfflineDataModel
            {
                RequestId = request.RequestId,
                Workload = request.Workload
            };
            var rsp = await _usageService.SetOfflineCourtTypeAsync(model);
            return Ok(new { IsValid = rsp });
        }

        [HttpPost("process-offline-status")]
        public IActionResult ProcessOfflineStatus(BulkReadResponse request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var find = offlineRequests.FirstOrDefault(x => x.OfflineRequestId == request.OfflineRequestId);
            if (find == null) return Unauthorized();
            if (find.IsCompleted)
            {
                offlineRequests.RemoveAll(x => x.OfflineRequestId == request.OfflineRequestId);
            }
            return Ok(find);
        }

        [HttpPost("get-offline-download-status")]
        public async Task<IActionResult> GetDownloadStatusAsync(BulkReadRequest request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var model = new OfflineDataModel
            {
                RequestId = request.RequestId
            };
            var rsp = await _usageService.GetDownloadStatusAsync(model);
            return Ok(new { request.RequestId, Content = rsp });
        }
        [HttpPost("set-offline-download-complete")]
        public async Task<IActionResult> SetDownloadCompletedAsync(OfflineDataModel request)
        {
            var user = _leadService.GetUserModel(Request, UserAccountAccess);
            if (user == null) return Unauthorized();
            var rsp = await _usageService.SetDownloadCompletedAsync(request);
            return Ok(new { request.RequestId, Content = rsp });
        }
        private void StatusChanged(object? sender, BulkReadMessages e)
        {
            var find = offlineRequests.FirstOrDefault(x => x.OfflineRequestId == e.OfflineRequestId);
            if (find == null) return;
            find.TotalProcessed = e.TotalProcessed;
            find.RecordCount = e.RecordCount;
            find.Messages.Clear();
            find.Messages.AddRange(e.Messages);
            if (string.IsNullOrEmpty(e.Workload) || e.TotalProcessed == 0) { return; }
            var isCompleted = e.TotalProcessed == e.RecordCount && e.RecordCount > 0;
            var model = new OfflineDataModel
            {
                RequestId = find.RequestId, // e.OfflineRequestId,
                Message = string.Join(Environment.NewLine, e.Messages),
                Workload = e.Workload,
                RowCount = isCompleted ? e.TotalProcessed - 1 : e.TotalProcessed,
                RetryCount = e.RetryCount,
            };
            _ = Task.Run(async () =>
            {
                await _usageService.UpdateOfflineRecordAsync(model);
                if (isCompleted)
                {
                    model.RowCount = e.TotalProcessed;
                    await _usageService.UpdateOfflineRecordAsync(model);
                    await _usageService.UpdateOfflineHistoryAsync();
                }
            });
        }

        private void StatusTerminated(object? sender, BulkReadMessages e)
        {
            var messages = new string[] {
                "Process terminated due to server timeout",
                $"Total records processed: {e.TotalProcessed}",
                $"Total retries {e.RetryCount}"
            };
            var model = new OfflineDataModel
            {
                RequestId = e.OfflineRequestId,
                Message = string.Empty,
                Workload = string.Join(Environment.NewLine, messages),
                RowCount = e.TotalProcessed,
                RetryCount = e.RetryCount,
            };
            _ = Task.Run(async () =>
            {
                await _usageService.TerminateOfflineRequestAsync(model);
            });
        }
        private const string UserAccountAccess = "user account access credential";
        private readonly static CultureInfo _culture = new("en-us");
        private readonly static List<BulkReadResponse> offlineRequests = [];


        private class CookieModel
        {

            public string Name { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Domain { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string SameSite { get; set; } = string.Empty;
            public string Expiry { get; set; } = string.Empty;

            public OpenQA.Selenium.Cookie Cookie()
            {
                var hasDate = DateTime.TryParse(Expiry, CultureInfo.CurrentCulture, out var date);
                DateTime? expirationDt = hasDate ? date : null;
                return new OpenQA.Selenium.Cookie(Name, Value, Domain, Path, expirationDt, false, false, SameSite);
            }
        }
    }
}
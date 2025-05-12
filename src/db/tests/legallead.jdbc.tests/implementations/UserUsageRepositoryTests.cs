using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Moq;
using Newtonsoft.Json;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class UserUsageRepositoryTests
    {
        // disabling warning for null reference in mock response
        // this is needed to simulate a null db response
        #pragma warning disable CS8604 // Possible null reference argument.

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanAppendUsageRecordAsync(int conditionId)
        {
            var dto = conditionId == 0 ? null : appendfaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyAppendLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyAppendLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.AppendUsageRecord(new jdbc.models.UserUsageAppendRecordModel());
            mock.Verify(x => x.QuerySingleOrDefaultAsync<DbCountyAppendLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public async Task RepoCanCompleteUsageRecordAsync(int conditionId)
        {
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            _ = await service.CompleteUsageRecord(new());
            mock.Verify(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void RepoCompleteUsageRecordSerializationCheck(int conditionId)
        {
            var model = new UserUsageCompleteRecordModel
            {
                UsageRecordId = "123-456-789",
                RecordCount = 5,
                ExcelName = "excelfile.xlsx",
                Password = "abcd1234!"
            };
            var fields = new string[]{
                "\"idx\":",
                "\"rc\":",
                "\"exlname\":",
                "\"pwd\":"
            };
            var find = fields[conditionId];
            var json = JsonConvert.SerializeObject(model);
            Assert.Contains(find, json);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public async Task RepoCanGetMonthlyLimitByIdAsync(int conditionId)
        {
            var dto = conditionId <= 0 ? null : usagefaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetMonthlyLimit("");
            mock.Verify(x => x.QueryAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanGetMonthlyLimitByCountyIdAsync(int conditionId)
        {
            var dto = conditionId <= 0 ? null : usagefaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetMonthlyLimit("", 0);
            mock.Verify(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(25, true)]
        [InlineData(30)]
        public async Task RepoCanGetUsageAsync(int conditionId, bool isMonthlyRequest = false)
        {
            var dto = conditionId < 0 ? null : recordfaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var leadId = provider.LeadId;
            var searchDate = provider.SearchDate;

            var names = exlfaker.Generate(10);
            if (conditionId != 30)
            {
                mock.Setup(x => x.QueryAsync<DbExcelNameDto>(
                        It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>())).ReturnsAsync(names);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbExcelNameDto>(
                        It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);

            }

            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<DbCountyUsageRequestDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbCountyUsageRequestDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetUsage(leadId, searchDate, isMonthlyRequest);
            mock.Verify(x => x.QueryAsync<DbCountyUsageRequestDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(25, true)]
        public async Task RepoCanGetUsageSummaryAsync(int conditionId, bool isMonthlyRequest = false)
        {
            const string prcExcel = "CALL USP_LEADUSER_EXL_GET_FILENAMES_BY_USERID ( ? );";
            var summaries = new[] { "CALL USP_USER_USAGE_GET_SUMMARY_MTD ( ?, ? );", "CALL USP_USER_USAGE_GET_SUMMARY_YTD ( ?, ? );" };
            var dto = conditionId < 0 ? null : summaryfaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var leadId = provider.LeadId;
            var searchDate = provider.SearchDate;
            var names = exlfaker.Generate(10);
            mock.Setup(x => x.QueryAsync<DbExcelNameDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(prcExcel)),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(names);

            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<DbUsageSummaryDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => summaries.Contains(s)),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbUsageSummaryDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => summaries.Contains(s)),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetUsageSummary(leadId, searchDate, isMonthlyRequest);
            mock.Verify(x => x.QueryAsync<DbUsageSummaryDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => summaries.Contains(s)),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanSetMonthlyLimitAsync(int conditionId)
        {
            var dto = conditionId <= 0 ? null : usagefaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var leadId = provider.LeadId;
            var countyId = provider.CountyId;
            var limit = provider.MonthlyLimit;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.SetMonthlyLimit(leadId, countyId, limit);
            mock.Verify(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path id returned
        [InlineData(1)] // return null
        public async Task RepoCanOfflineRequestBeginAsync(int conditionId)
        {
            var guid = Guid.NewGuid().ToString("D");
            var dto = conditionId < 0 ? null : new OfflineBeginDto { Id = guid };
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = new OfflineRequestModel();

            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<OfflineBeginDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<OfflineBeginDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.OfflineRequestBeginAsync(rqst);
            mock.Verify(x => x.QuerySingleOrDefaultAsync<OfflineBeginDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path 
        public async Task RepoCanOfflineRequestUpdateAsync(int conditionId)
        {
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = new OfflineRequestModel();

            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            _ = await service.OfflineRequestUpdateAsync(rqst);
            mock.Verify(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path id returned
        [InlineData(1)] // return null
        public async Task RepoCanGetOfflineStatusAsync(int conditionId)
        {
            var dto = conditionId == 1 ? null : offlinefaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = new OfflineRequestModel { RequestId = "testing" };

            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetOfflineStatusAsync(rqst);
            mock.Verify(x => x.QuerySingleOrDefaultAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path id returned
        [InlineData(1)] // return null
        [InlineData(2)] // return empty
        public async Task RepoCanGetOfflineStatuByIdAsync(int conditionId)
        {
            var dto = conditionId switch
            {
                1 => default,
                2 => [],
                _ => offlinefaker.Generate(5)
            };
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = "testing";

            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetOfflineStatusAsync(rqst);
            mock.Verify(x => x.QueryAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path id returned
        [InlineData(1)] // return null
        [InlineData(2)] // return empty
        public async Task RepoCanGetOfflineGetSearchTypeAsync(int conditionId)
        {
            var dto = conditionId switch
            {
                1 => default,
                2 => [],
                _ => searchtypefaker.Generate(5)
            };
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = "testing";

            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<OfflineSearchTypeDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<OfflineSearchTypeDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetOfflineGetSearchTypeAsync(rqst);
            mock.Verify(x => x.QueryAsync<OfflineSearchTypeDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path 
        public async Task RepoCanOfflineRequestTerminateAsync(int conditionId)
        {
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = new OfflineRequestModel();

            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            _ = await service.OfflineRequestTerminateAsync(rqst);
            mock.Verify(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path 
        public async Task RepoCanOfflineRequestSetCourtTypeAsync(int conditionId)
        {
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = new OfflineRequestModel();

            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            _ = await service.OfflineRequestSetCourtTypeAsync(rqst);
            mock.Verify(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path id returned
        [InlineData(1)] // return null
        public async Task RepoCanOfflineRequestCanDownload(int conditionId)
        {
            var dto = conditionId == 1 ? null : downloadfaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = new OfflineRequestModel { RequestId = "testing" };

            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<OfflineDownloadDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<OfflineDownloadDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.OfflineRequestCanDownload(rqst);
            mock.Verify(x => x.QuerySingleOrDefaultAsync<OfflineDownloadDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path 
        [InlineData(1)] // id is empty
        [InlineData(11)] // id is not a guid
        [InlineData(2)] // request id is empty
        [InlineData(22)] // request id is not a guid
        [InlineData(3)] // workload is empty
        [InlineData(4)] // can download is false 
        public async Task RepoCanOfflineRequestFlagAsDownloadedAsync(int conditionId)
        {
            var model = downloadmodelfaker.Generate();
            model.CanDownload = conditionId != 4;
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId == 1) model.Id = string.Empty;
            if (conditionId == 11) model.Id = "not-a-guid";
            if (conditionId == 2) model.RequestId = string.Empty;
            if (conditionId == 22) model.RequestId = "not-a-guid";
            if (conditionId == 3) model.Workload = string.Empty;
            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            var actual = await service.OfflineRequestFlagAsDownloadedAsync(model);
            Assert.Equal(conditionId == 0, actual);
            if (conditionId <= 0)
                mock.Verify(x => x.ExecuteAsync(
                        It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path 
        public async Task RepoCanOfflineRequestSetSearchTypeAsync(int conditionId)
        {
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            _ = await service.OfflineRequestSetSearchTypeAsync();
            mock.Verify(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path 
        public async Task RepoCanOfflineRequestSyncHistoryAsync(int conditionId)
        {
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            _ = await service.OfflineRequestSyncHistoryAsync();
            mock.Verify(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path id returned
        [InlineData(1)] // return null
        public async Task RepoCanOfflineFindByCaseNumber(int conditionId)
        {
            var dto = conditionId == 1 ? null : caseitemfaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var rqst = new OfflineCaseItemModel { CountyId = 0, CaseNumber = "testing" };

            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<OfflineCaseItemDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<OfflineCaseItemDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.OfflineFindByCaseNumber(rqst);
            mock.Verify(x => x.QuerySingleOrDefaultAsync<OfflineCaseItemDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(-1)] // exception
        [InlineData(0)] // happy path id returned
        [InlineData(1)] // return null
        [InlineData(2)] // return empty
        public async Task RepoCanGetOfflineWorkQueueAsync(int conditionId)
        {
            var dto = conditionId switch
            {
                1 => default,
                2 => [],
                _ => offlinefaker.Generate(5)
            };
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;

            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetOfflineWorkQueueAsync();
            mock.Verify(x => x.QueryAsync<OfflineStatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }
#pragma warning restore CS8604 // Possible null reference argument.
        private sealed class RepoContainer
        {
            private readonly IUserUsageRepository repo;

            public Exception Error { get; private set; }

            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                var fkr = new Faker();
                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new UserUsageRepository(dataContext);
                Error = fkr.System.Exception();
                LeadId = fkr.Random.Guid().ToString();
                SearchDate = fkr.Date.Past();
                CountyId = fkr.Random.Int(1, 1000);
                MonthlyLimit = fkr.Random.Int(100, 10000);
            }

            public IUserUsageRepository Repository => repo;
            public Mock<IDapperCommand> DbCommandMock => command;
            public string LeadId { get; private set; }
            public DateTime SearchDate { get; private set; }
            public int CountyId { get; private set; }
            public int MonthlyLimit { get; private set; }
        }

        private static readonly Faker<DbCountyAppendLimitDto> appendfaker =
            new Faker<DbCountyAppendLimitDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"));

        private static readonly Faker<DbCountyUsageLimitDto> usagefaker
            = new Faker<DbCountyUsageLimitDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.MaxRecords, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent());

        private static readonly Faker<DbCountyUsageRequestDto> recordfaker
            = new Faker<DbCountyUsageRequestDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.DateRange, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent());

        private static readonly Faker<DbUsageSummaryDto> summaryfaker
            = new Faker<DbUsageSummaryDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.UserName, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.SearchYear, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.SearchMonth, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.LastSearchDate, y => y.Date.Recent())
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.MTD, y => y.Random.Int())
            .RuleFor(x => x.MonthlyLimit, y => y.Random.Int(1, 555555));

        private static readonly Faker<DbExcelNameDto> exlfaker
            = new Faker<DbExcelNameDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.ShortFileName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.FileToken, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker<OfflineStatusDto> offlinefaker
            = new Faker<OfflineStatusDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RequestId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Workload, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Cookie, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Message, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.OfflineId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RowCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.RetryCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.SearchStartDate, y => y.Date.Recent())
            .RuleFor(x => x.SearchEndDate, y => y.Date.Recent())
            .RuleFor(x => x.PercentComplete, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.LastUpdate, y => y.Date.Recent());

        private static readonly Faker<OfflineDownloadDto> downloadfaker
            = new Faker<OfflineDownloadDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RequestId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Workload, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CanDownload, y => y.Random.Bool());

        private static readonly Faker<OfflineDownloadModel> downloadmodelfaker
            = new Faker<OfflineDownloadModel>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.RequestId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Workload, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CanDownload, y => true);

        private static readonly Faker<OfflineSearchTypeDto> searchtypefaker
            = new Faker<OfflineSearchTypeDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 2600))
            .RuleFor(x => x.SearchType, y => y.Random.AlphaNumeric(25));

        private static readonly Faker<OfflineCaseItemDto> caseitemfaker
            = new Faker<OfflineCaseItemDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 2600))
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.CaseHeader, y => y.Random.AlphaNumeric(150))
            .RuleFor(x => x.Plaintiff, y => y.Random.AlphaNumeric(150))
            .RuleFor(x => x.Address, y => y.Random.AlphaNumeric(150))
            .RuleFor(x => x.Zip, y => y.Random.AlphaNumeric(5))
            .RuleFor(x => x.PersonName, y => y.Random.AlphaNumeric(25));
    }
}
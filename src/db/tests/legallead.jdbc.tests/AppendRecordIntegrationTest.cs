using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.models;
using System.Diagnostics;

namespace legallead.jdbc.tests
{
    public class AppendRecordIntegrationTest
    {
        [Fact]
        public async Task ServiceAppendCheckAsync()
        {
            if (!Debugger.IsAttached) return;
            var service = GetRepo();
            var payload = new UserUsageAppendRecordModel
            {
                LeadUserId = "fef29532-a487-11ef-99ce-0af7a01f52e9",
                CountyId = 60,
                CountyName = "Dallas",
                StartDate = new DateTime(2024, 12, 12, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2024, 12, 12, 0, 0, 0, DateTimeKind.Local)
            };
            var actual = await service.AppendUsageRecord(payload);
            Assert.NotNull(actual);
        }


        [Fact]
        public async Task ServiceGetMonthlyLimitByIdCheckAsync()
        {
            if (!Debugger.IsAttached) return;
            var service = GetRepo();
            var payload = new
            {
                LeadId = "fef29532-a487-11ef-99ce-0af7a01f52e9"
            };
            var actual = await service.GetMonthlyLimit(payload.LeadId);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async Task ServiceGetMonthlyLimitCheckAsync()
        {
            if (!Debugger.IsAttached) return;
            var service = GetRepo();
            var payload = new
            {
                LeadId = "fef29532-a487-11ef-99ce-0af7a01f52e9",
                CountyId = 60
            };
            var actual = await service.GetMonthlyLimit(payload.LeadId, payload.CountyId);
            Assert.NotNull(actual);
            Assert.False(string.IsNullOrEmpty(actual.Id));
        }


        [Fact]
        public async Task ServiceGetMonthlyDetailByIdCheckAsync()
        {
            if (!Debugger.IsAttached) return;
            var service = GetRepo();
            var payload = new
            {
                LeadId = "fef29532-a487-11ef-99ce-0af7a01f52e9",
                SearchDate = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Local)
            };
            var actual = await service.GetUsage(payload.LeadId, payload.SearchDate, true);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.True(actual.Count > 50);
        }


        [Fact]
        public async Task ServiceGetAnnualDetailByIdCheckAsync()
        {
            if (!Debugger.IsAttached) return;
            var service = GetRepo();
            var payload = new
            {
                LeadId = "fef29532-a487-11ef-99ce-0af7a01f52e9",
                SearchDate = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Local)
            };
            var actual = await service.GetUsage(payload.LeadId, payload.SearchDate);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.True(actual.Count > 100);
        }


        [Fact]
        public async Task ServiceGetMonthlySummaryByIdCheckAsync()
        {
            if (!Debugger.IsAttached) return;
            var service = GetRepo();
            var payload = new
            {
                LeadId = "fef29532-a487-11ef-99ce-0af7a01f52e9",
                SearchDate = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Local)
            };
            var actual = await service.GetUsageSummary(payload.LeadId, payload.SearchDate, true);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.True(actual.Count > 5);
        }


        [Fact]
        public async Task ServiceGetAnnualSummaryByIdCheckAsync()
        {
            if (!Debugger.IsAttached) return;
            var service = GetRepo();
            var payload = new
            {
                LeadId = "fef29532-a487-11ef-99ce-0af7a01f52e9",
                SearchDate = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Local)
            };
            var actual = await service.GetUsageSummary(payload.LeadId, payload.SearchDate);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.True(actual.Count > 10);
        }

        private static UserUsageRepository GetRepo()
        {
            var command = new DapperExecutor();
            return new UserUsageRepository(new(command));
        }
    }
}

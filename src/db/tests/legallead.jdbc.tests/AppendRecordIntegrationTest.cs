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
            var command = new DapperExecutor();
            var service = new UserUsageRepository(new(command));
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
    }
}

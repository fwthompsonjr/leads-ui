// See https://aka.ms/new-console-template for more information
// assert that provider can get instance of ILeadUserRepository


using db.integration.tests;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
int testId = 11;
var invoicing = ServiceSetup.AppServices.GetRequiredService<IInvoiceRepository>();
if (testId == 0)
{
    var instance = ServiceSetup.AppServices.GetRequiredService<ILeadUserRepository>();
    var account = TestAccountProvider.GetChangeCountyIndex();
    var actual = await instance.UpdateCountyPermissions(account);
    Console.WriteLine("Hello, World! {0}. Db result {1}", instance.GetType().FullName, actual);
}
if (testId == 1)
{
    var actual = await invoicing.GenerateInvoicesAsync();
    Console.WriteLine("Hello, World! {0}. Db result {1}", invoicing.GetType().FullName, actual);
}
if (testId == 2)
{
    var actual = await invoicing.FindAllAsync();
    var count = actual?.Count ?? 0;
    Console.WriteLine("Hello, World! {0}. Db result {1}", invoicing.GetType().FullName, count);
}
if (testId == 3)
{
    var findByUser = new DbInvoiceViewBo { UserName = "lead.administrator", LineNbr = 1 };
    var actual = await invoicing.QueryAsync(findByUser);
    var count = actual?.Count ?? 0;
    Console.WriteLine("Hello, World! {0}. Db result {1}", invoicing.GetType().FullName, count);
}
if (testId == 4)
{
    var findByIndex = new DbInvoiceViewBo { RequestId = "cd2fef42-bb59-11ef-99ce-0af7a01f52e9" };
    var actual = await invoicing.QueryAsync(findByIndex);
    var count = actual?.Count ?? 0;
    Console.WriteLine("Hello, World! {0}. Db result {1}", invoicing.GetType().FullName, count);
}
if (testId == 5)
{
    var updateStatus = new DbInvoiceViewBo
    {
        Id = "36ca6adf-c0b7-11ef-b422-0af36f7c981d",
        InvoiceNbr = "SENT"
    };
    var actual = await invoicing.UpdateAsync(updateStatus);
    var count = actual.Key;
    Console.WriteLine("Hello, World! {0}. Db result {1}", invoicing.GetType().FullName, count);
}
if (testId == 6)
{
    var updateUri = new DbInvoiceViewBo
    {
        Id = "36ca6adf-c0b7-11ef-b422-0af36f7c981d",
        InvoiceUri = "http://www.google.com"
    };
    var actual = await invoicing.UpdateAsync(updateUri);
    var count = actual.Key;
    Console.WriteLine("Hello, World! {0}. Db result {1}", invoicing.GetType().FullName, count);
}
if (testId == 7)
{
    var updateComplete = new DbInvoiceViewBo
    {
        Id = "36ca6adf-c0b7-11ef-b422-0af36f7c981d",
        InvoiceUri = "http://www.completed.com",
        InvoiceNbr = "PAID",
        CompleteDate = DateTime.UtcNow.Date
    };
    var actual = await invoicing.UpdateAsync(updateComplete);
    var count = actual.Key;
    Console.WriteLine("Hello, World! {0}. Db result {1}", invoicing.GetType().FullName, count);
}
if (testId == 8)
{
    var instance = ServiceSetup.AppServices.GetRequiredService<IUserUsageRepository>();
    var leadId = "fef29532-a487-11ef-99ce-0af7a01f52e9";
    var dte = new DateTime(2024, 11, 1, 0, 0, 0, DateTimeKind.Utc);
    var actual = await instance.GetUsage(leadId, dte);
    Console.WriteLine("Hello, World! {0}. Db result {1}", instance.GetType().FullName, actual);
}
if (testId == 9)
{
    var instance = ServiceSetup.AppServices.GetRequiredService<IUserUsageRepository>();
    var leadId = "fef29532-a487-11ef-99ce-0af7a01f52e9";
    var dte = new DateTime(2024, 11, 1, 0, 0, 0, DateTimeKind.Utc);
    var actual = await instance.GetUsageSummary(leadId, dte);
    Console.WriteLine("Hello, World! {0}. Db result {1}", instance.GetType().FullName, actual);
}
if (testId == 10)
{
    var request = new DbCountyFileModel
    {
        FileContent = "integration test data",
        Id = "5711f02d-c401-11ef-b422-0af36f7c981d"
    };
    var instance = ServiceSetup.AppServices.GetRequiredService<ICountyFileRepository>();
    var isInitialized = await instance.InitializeAsync();
    Console.WriteLine("Initialization response: {0}", isInitialized);
    // update types
    var typeNames = new string[] { "EXL", "CSV", "JSON", "NONE" };
    for (int i = 0; i < typeNames.Length; i++)
    {
        var type = typeNames[i];
        request.FileType = type;
        var isTypeSet = await instance.UpdateTypeAsync(request);
        Console.WriteLine("Type update response: {0}: {1}", isTypeSet.Key, isTypeSet.Value);
    }
    // update status
    var statusNames = new string[] { "ENCODED", "DECODED", "DOWNLOADED", "EMPTY" };
    for (int i = 0; i < statusNames.Length; i++)
    {
        var status = statusNames[i];
        request.FileStatus = status;
        var isStatusSet = await instance.UpdateStatusAsync(request);
        Console.WriteLine("Status update response: {0}: {1}", isStatusSet.Key, isStatusSet.Value);
    }
    // update content
    for (int i = 0; i < 2; i++)
    {
        if (i > 0) request.FileContent = "";
        var isContentSet = await instance.UpdateContentAsync(request);
        Console.WriteLine("Content update response: {0}: {1}", isContentSet.Key, isContentSet.Value);
    }
}
if (testId == 11)
{
    var svc = ServiceSetup.AppServices.GetRequiredService<IUserUsageRepository>();
    var logs = JsonConvert.SerializeObject(new List<string>
    {
        "This log is your log",
        "Cant be updated",
    });
    var tmp = JsonConvert.SerializeObject(new List<string>
    {
        "record 1",
        "record 2",
    });
    var model = new OfflineRequestModel
    {
        OfflineId = "fef29532-a487-11ef-99ce-0af7a01f52e9",
        RequestId = "44b807b9-1d35-11f0-b422-0af36f7c981d",
        RowCount = 16, 
        RetryCount = 2,
        Message = logs,
        Workload = tmp,
    };
    var actual = await svc.OfflineRequestCanDownload(model) ?? new();
    if (!string.IsNullOrEmpty(actual.Workload))
        actual.Workload = "<!-- removed -->";
    var js = JsonConvert.SerializeObject(actual, Formatting.Indented);
    Console.WriteLine(js);
}
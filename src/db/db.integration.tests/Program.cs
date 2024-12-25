// See https://aka.ms/new-console-template for more information
// assert that provider can get instance of ILeadUserRepository


using db.integration.tests;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using Microsoft.Extensions.DependencyInjection;
int testId = 7;
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
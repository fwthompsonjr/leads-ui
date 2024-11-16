// See https://aka.ms/new-console-template for more information
// assert that provider can get instance of ILeadUserRepository


using db.integration.tests;
using legallead.jdbc.interfaces;
using Microsoft.Extensions.DependencyInjection;

var instance = ServiceSetup.AppServices.GetRequiredService<ILeadUserRepository>();
Console.WriteLine("Hello, World! {0}", instance.GetType().FullName);

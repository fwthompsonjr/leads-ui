using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.search.api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped(x =>
{
    var command = new DapperExecutor();
    return new DataContext(command);
});
services.AddScoped<IUserSearchRepository, UserSearchRepository>(x =>
{
    var context = x.GetRequiredService<DataContext>();
    return new UserSearchRepository(context);
});
services.AddScoped(x =>
{
    var context = x.GetRequiredService<IUserSearchRepository>();
    return new ApiController(context);
});
services.AddSingleton(s => { return s; });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

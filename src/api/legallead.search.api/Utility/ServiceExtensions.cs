using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.search.api.Controllers;

namespace legallead.search.api.Utility
{
    public static class ServiceExtensions
    {
        public static void Initialize(this IServiceCollection services)
        {
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
            services.AddScoped<ISearchQueueRepository, SearchQueueRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                return new SearchQueueRepository(context);
            });
            services.AddScoped(x =>
            {
                var context = x.GetRequiredService<IUserSearchRepository>();
                return new ApiController(context);
            });
            services.AddSingleton(s => { return s; });
        }
    
        public static void Initialize(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

        }
    }
}

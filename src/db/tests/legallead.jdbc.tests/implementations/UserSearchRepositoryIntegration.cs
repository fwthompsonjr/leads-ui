using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace legallead.jdbc.tests.implementations
{
    public class UserSearchRepositoryIntegration
    {
        private readonly IUserSearchRepository _repo;
        public UserSearchRepositoryIntegration()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IDapperCommand, DapperExecutor>();
            services.AddSingleton(x =>
            {
                var exec = x.GetRequiredService<IDapperCommand>();
                return new DataContext(exec);
            });
            services.AddSingleton<IUserSearchRepository, UserSearchRepository>();
            var provider = services.BuildServiceProvider();
            _repo = provider.GetRequiredService<IUserSearchRepository>();
        }

        [Fact]
        public void RepositoryIsNotNull()
        {
            Assert.NotNull(_repo);
        }

        [Fact]
        public async Task RepoCanGetHistory()
        {
            if (!Debugger.IsAttached) return;
            const string userid = "cf35094a-ad64-41dd-9f2d-32cbc942aaed";
            var result = await _repo.History(userid);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}

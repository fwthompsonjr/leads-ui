using legallead.desktop.entities;
using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.tests.implementations
{
    public class SearchBuilderTests
    {
        [Fact]
        public void BuilderCanBeConstructed()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchBuilder(GetApi());
            });
            Assert.Null(exception);
        }

        [Fact]
        public void BuilderCanGetAll()
        {
            var exception = Record.Exception(() =>
            {
                var sut = new SearchBuilder(GetApi());
                for (var i = 0; i < 2; i++)
                {
                    _ = sut.GetConfiguration();
                }
            });
            Assert.Null(exception);
        }

        [Fact]
        public void BuilderCanGetHtml()
        {
            var exception = Record.Exception(() =>
            {
                var sut = new SearchBuilder(GetApi());
                var html = sut.GetHtml();
                Assert.False(string.IsNullOrEmpty(html));
            });
            Assert.Null(exception);
        }

        private static IPermissionApi GetApi()
        {
            return new MyMockApi();
        }

        private sealed class MyMockApi : PermissionApi
        {
            private static readonly IInternetStatus internetStatus = new ActiveInternetStatus();

            public MyMockApi() : base(string.Empty, internetStatus)
            {
            }

            public override async Task<ApiResponse> Get(string name)
            {
                return await Task.Run(() =>
                {
                    return new ApiResponse
                    {
                        Message = Properties.Resources.state_config_response,
                        StatusCode = 200
                    };
                });
            }
        }

        private sealed class ActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }
    }
}
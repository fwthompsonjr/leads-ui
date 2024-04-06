using legallead.installer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.installer.tests
{
    public class RepositoryStorageTest
    {
        [Fact]
        public void SutCanBeCreated()
        {
            var exception = Record.Exception(() => { 
                var item = new RepositoryStorage(); 
                Assert.False(string.IsNullOrEmpty(item.Name));
            });
            Assert.Null(exception);
        }
    }
}

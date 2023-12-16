using System.Reflection;
using System.Resources;

namespace legallead.desktop.tests
{
    public class GetResourceTest
    {
        [Fact]
        public void AssemblyCanGetIntroductionText()
        {
            var name = GetResourceName();
            var assembly = GetTargetAssembly();
            var info = assembly.GetManifestResourceInfo(name);
            Assert.NotNull(info);
        }

        private string GetResourceName()
        {
            var assembly = GetTargetAssembly();
            var resources = assembly.GetManifestResourceNames();
            if (resources == null || !resources.Any()) return string.Empty;
            return resources[0];
        }

        private Assembly GetTargetAssembly()
        {
            var type = typeof(GetResourceTest);
            return type.Assembly;
        }
    }
}
using legallead.installer.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Bo
{
    [ExcludeFromCodeCoverage(Justification = "Item scheduled for deletion")]
    internal class ModelStorageBo<T> : ICreateDateProperty where T : class
    {

        public string CreationDate { get; set; } = string.Empty;
        public List<T> Detail { get; set; } = [];
        public string Name { get; set; } = string.Empty;
    }
}

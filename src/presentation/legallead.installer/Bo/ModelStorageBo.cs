using legallead.installer.Interfaces;

namespace legallead.installer.Bo
{
    internal class ModelStorageBo<T> : ICreateDateProperty where T : class
    {

        public string CreationDate { get; set; } = string.Empty;
        public List<T> Detail { get; set; } = [];
        public string Name { get; set; } = string.Empty;
    }
}

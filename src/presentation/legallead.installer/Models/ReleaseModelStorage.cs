using legallead.installer.Interfaces;

namespace legallead.installer.Models
{
    public class ReleaseModelStorage : ModelStorageBase<ReleaseModel>
    {
        public override string Name => "Git.Releases";
    }
}

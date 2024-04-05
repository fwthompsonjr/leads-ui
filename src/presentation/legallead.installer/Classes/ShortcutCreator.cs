using IWshRuntimeLibrary;
using legallead.installer.Interfaces;
using legallead.installer.Models;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Classes
{
    public class ShortcutCreator : IShortcutCreator
    {
        [ExcludeFromCodeCoverage(Justification = "Process creates file resources, integration testing only")]
        public void Build(ReleaseAssetModel model, string targetDir, FileInfo executableFile)
        {
            var linkFile = $"{model.Name}-{model.Version}.lnk";
            WshShell shell = new();
            string shortcutAddress = Path.Combine(targetDir, linkFile);
            if (System.IO.File.Exists(shortcutAddress)) { System.IO.File.Delete(shortcutAddress); }
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = $"Shortcut Legal Lead : {model.Name}-{model.Version}";
            shortcut.TargetPath = executableFile.FullName;
            shortcut.WorkingDirectory = Path.GetDirectoryName(executableFile.FullName);
            shortcut.Save();
        }
    }
}

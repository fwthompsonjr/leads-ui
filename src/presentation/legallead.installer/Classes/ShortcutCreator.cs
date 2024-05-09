using legallead.installer.Interfaces;
using legallead.installer.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace legallead.installer.Classes
{
    [ExcludeFromCodeCoverage(Justification = "Process creates file resources, integration testing only")]
    public class ShortcutCreator : IShortcutCreator
    {
#pragma warning disable CS8602 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static readonly Type? m_type = Type.GetTypeFromProgID("WScript.Shell");
        private static readonly object? m_shell = Activator.CreateInstance(m_type);

        public void Install(IShortcutCreator service, ReleaseAssetModel item, string installPath, string name, string version)
        {

            for (var sc = 0; sc < 2; sc++)
            {
                var messagetype = sc == 0 ? "application" : "desktop";
                var message = $"Creating {messagetype} shortcut: {name} version: {version}.";
                Console.WriteLine(message);
                var linkAddress = service.Create(item, installPath, sc == 1);
                if (sc != 1) continue;
                // launch the shortcut
                LaunchShortCut(linkAddress);
            }
        }

        public string? Build(ReleaseAssetModel model, string targetDir, FileInfo executableFile)
        {
            try
            {
                var linkFile = $"{model.Name}-{model.Version}.lnk";
                string shortcutAddress = Path.Combine(targetDir, linkFile);
                var workingDirectory = Path.GetDirectoryName(executableFile.FullName);
                if (string.IsNullOrEmpty(workingDirectory)) return null;
                if (File.Exists(shortcutAddress)) { File.Delete(shortcutAddress); }
                IWshShortcut shortcut = (IWshShortcut)m_type.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, m_shell, new object[] { shortcutAddress });

                if (shortcut == null) return null;
                shortcut.Description = $"Shortcut Legal Lead : {model.Name}-{model.Version}";
                shortcut.TargetPath = executableFile.FullName;
                shortcut.WorkingDirectory = workingDirectory;
                shortcut.Save();
                return shortcutAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        [ExcludeFromCodeCoverage(Justification = "Interacts with file system. Tested in integration")]
        private static void LaunchShortCut(string linkAddress)
        {
            if (string.IsNullOrEmpty(linkAddress)) return;
            if (!File.Exists(linkAddress)) return;
            try
            {
                Process proc = new();
                proc.StartInfo.FileName = linkAddress;
                proc.Start();
            }
            catch (Exception)
            {
                // take no action on failure here
                return;
            }
        }

        [ComImport, TypeLibType((short)0x1040), Guid("F935DC23-1CF0-11D0-ADB9-00C04FD58A0B")]
        private interface IWshShortcut
        {
            [DispId(0)]
            string FullName { [return: MarshalAs(UnmanagedType.BStr)][DispId(0)] get; }
            [DispId(0x3e8)]
            string Arguments { [return: MarshalAs(UnmanagedType.BStr)][DispId(0x3e8)] get; [param: In, MarshalAs(UnmanagedType.BStr)][DispId(0x3e8)] set; }
            [DispId(0x3e9)]
            string Description { [return: MarshalAs(UnmanagedType.BStr)][DispId(0x3e9)] get; [param: In, MarshalAs(UnmanagedType.BStr)][DispId(0x3e9)] set; }
            [DispId(0x3ea)]
            string Hotkey { [return: MarshalAs(UnmanagedType.BStr)][DispId(0x3ea)] get; [param: In, MarshalAs(UnmanagedType.BStr)][DispId(0x3ea)] set; }
            [DispId(0x3eb)]
            string IconLocation { [return: MarshalAs(UnmanagedType.BStr)][DispId(0x3eb)] get; [param: In, MarshalAs(UnmanagedType.BStr)][DispId(0x3eb)] set; }
            [DispId(0x3ec)]
            [SuppressMessage("Code Smell", "S2376:Write-only properties should not be used", Justification = "Following pattern for COM interface")]
            string RelativePath { [param: In, MarshalAs(UnmanagedType.BStr)][DispId(0x3ec)] set; }
            [DispId(0x3ed)]
            string TargetPath { [return: MarshalAs(UnmanagedType.BStr)][DispId(0x3ed)] get; [param: In, MarshalAs(UnmanagedType.BStr)][DispId(0x3ed)] set; }
            [DispId(0x3ee)]
            int WindowStyle { [DispId(0x3ee)] get; [param: In][DispId(0x3ee)] set; }
            [DispId(0x3ef)]
            string WorkingDirectory { [return: MarshalAs(UnmanagedType.BStr)][DispId(0x3ef)] get; [param: In, MarshalAs(UnmanagedType.BStr)][DispId(0x3ef)] set; }
            [TypeLibFunc((short)0x40), DispId(0x7d0)]
            void Load([In, MarshalAs(UnmanagedType.BStr)] string PathLink);
            [DispId(0x7d1)]
            void Save();
        }

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.

    }
}

using legallead.installer.Interfaces;
using legallead.installer.Models;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace legallead.installer.Classes
{
    [ExcludeFromCodeCoverage(Justification = "Process creates file resources, integration testing only")]
    public class ShortcutCreator : IShortcutCreator
    {
        private static readonly Type m_type = Type.GetTypeFromProgID("WScript.Shell");
        private static readonly object m_shell = Activator.CreateInstance(m_type);

        public void Install(IShortcutCreator service, ReleaseAssetModel item, string installPath, string name, string version)
        {

            for (var sc = 0; sc < 2; sc++)
            {
                var messagetype = sc == 0 ? "application" : "desktop";
                var message = $"Creating {messagetype} shortcut: {name} version: {version}.";
                Console.WriteLine(message);
                service.Create(item, installPath, sc == 1);
            }
        }

        public void Build(ReleaseAssetModel model, string targetDir, FileInfo executableFile)
        {
            try
            {

                var linkFile = $"{model.Name}-{model.Version}.lnk";
                string shortcutAddress = Path.Combine(targetDir, linkFile);
                var workingDirectory = Path.GetDirectoryName(executableFile.FullName);
                if (string.IsNullOrEmpty(workingDirectory)) return;
                if (File.Exists(shortcutAddress)) { File.Delete(shortcutAddress); }
                IWshShortcut shortcut = (IWshShortcut)m_type.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, m_shell, new object[] { shortcutAddress });
                if (shortcut == null) return;
                shortcut.Description = $"Shortcut Legal Lead : {model.Name}-{model.Version}";
                shortcut.TargetPath = executableFile.FullName;
                shortcut.WorkingDirectory = workingDirectory;
                shortcut.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
    }
}

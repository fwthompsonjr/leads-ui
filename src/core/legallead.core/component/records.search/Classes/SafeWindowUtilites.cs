using System.Runtime.InteropServices;

namespace legallead.records.search.Classes
{
    internal static class SafeWindowUtilites
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static extern int ShowWindow(int hwnd, int nCmdShow);
    }
}
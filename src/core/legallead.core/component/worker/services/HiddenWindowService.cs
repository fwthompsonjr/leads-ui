﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace legallead.reader.component.services
{
    internal partial class HiddenWindowService : IDisposable
    {
        [LibraryImport("User32")]
        private static partial int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", 
            "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", 
            Justification = "<Pending>")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private static readonly List<string> DriverServices = [
            "geckodriver",
            "firefox",
            "chromedriver",
            "IEDriverServer"
        ];
        private readonly System.Timers.Timer aTimer;
        private bool disposedValue;

        public HiddenWindowService()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(250);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            HideSelections();
        }

        private static void HideSelections()
        {
            const int sw_hide = 0;
            DriverServices.ForEach(s =>
            {
                var processes = Process.GetProcessesByName(s).ToList();
                foreach (var process in processes)
                {
                    var hwnd = process.MainWindowHandle.ToInt32();
                    var isVisible = IsWindowVisible(hwnd);
                    if (isVisible) { _ = ShowWindow(hwnd, sw_hide); }
                }
            });
        }
        private static void RestoreBrowserWindows()
        {
            const int sw_show = 5;
            const string firefox = "firefox";
            var processes = Process.GetProcessesByName(firefox).ToList();
            processes.ForEach(p =>
            {
                var hwnd = p.MainWindowHandle.ToInt32();
                var isVisible = IsWindowVisible(hwnd);
                if (!isVisible) { _ = ShowWindow(hwnd, sw_show); }
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    aTimer.Stop();
                    aTimer.Dispose();
                    RestoreBrowserWindows();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

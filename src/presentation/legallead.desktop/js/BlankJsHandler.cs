using CefSharp.Wpf;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace legallead.desktop.js
{
    internal class BlankJsHandler : JsHandler
    {
        public BlankJsHandler(ChromiumWebBrowser? browser) : base(browser)
        {
        }

        public override void OnPageLoaded()
        {
            Console.WriteLine("application base page is loading.");
            var minimun = TimeSpan.FromMilliseconds(500);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (stopWatch.Elapsed < minimun)
            {
                Thread.Sleep(100);
            }
            stopWatch.Stop();
            _ = Task.Run(() =>
            {
                OnInitCompleted?.Invoke(null);
            }).Wait(100);
        }
    }
}
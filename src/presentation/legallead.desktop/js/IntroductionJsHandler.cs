using System;
using System.Diagnostics;
using System.Threading;

namespace legallead.desktop.js
{
    internal class IntroductionJsHandler : JsHandler
    {
        public IntroductionJsHandler() : base()
        {
        }

        public override void Initialize()
        {
            Console.WriteLine("initialize remote data process is requested");
            var minimun = TimeSpan.FromSeconds(1.2);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Init();
            while (stopWatch.Elapsed < minimun)
            {
                Thread.Sleep(100);
            }
            stopWatch.Stop();
            OnInitCompleted?.Invoke(null);
        }
    }
}
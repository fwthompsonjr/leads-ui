using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.handlers
{
    public class CustomRequestHandler : CefSharp.Handler.RequestHandler
    {
        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            if (request.TransitionType == TransitionType.LinkClicked)
            {
                //Cancel the request by returning true 
                return true;
            }
            if (!string.IsNullOrEmpty(request.Url) && request.Url.StartsWith(InternalApi))
            {
                HandleAction(request.Url);
                return true;
            }
            return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
        }

        private void HandleAction(string url)
        {
            throw new NotImplementedException();
        }

        private static readonly string InternalApi = "http://internal.legalead.com/";
    }
}

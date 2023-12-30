using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;

namespace legallead.desktop.handlers
{
    internal class LoginFormHandler
    {
        private readonly ChromiumWebBrowser? _web;
        public int FormIndex { get; private set; }
        public string Payload { get; private set; }
        public bool IsValid => FormIndex >= 0;
        private readonly bool? HasWebBrowser;

        public LoginFormHandler(ChromiumWebBrowser? browser, string formName, string json)
        {
            var formNames = JsHomeFormSubmittedHandler.HomeFormNames;
            FormIndex = formNames.FindIndex(x => x.Equals(formName, StringComparison.OrdinalIgnoreCase));
            Payload = json;
            _web = browser;

            HasWebBrowser = IsValid && _web != null && _web.CanExecuteJavascriptInMainFrame;
        }

        public void Start()
        {
            if (!HasWebBrowser.GetValueOrDefault() || _web == null) return;

            scriptNames.ForEach(s =>
            {
                if (scriptNames.IndexOf(s) == 0)
                {
                    // setup animation so user knows process is running
                    _web.ExecuteScriptAsync(s, FormIndex, true);
                }
                if (scriptNames.IndexOf(s) == 1)
                {
                    // clear any previous submission messages
                    _web.ExecuteScriptAsync(s, FormIndex, "", false);
                }
            });
        }

        internal void SetMessage(string htm)
        {
            if (!HasWebBrowser.GetValueOrDefault()) return;
            _web.ExecuteScriptAsync(scriptNames[1], FormIndex, htm, true);
        }

        public void End()
        {
            if (!HasWebBrowser.GetValueOrDefault()) return;
            _web.ExecuteScriptAsync(scriptNames[0], FormIndex, false);
        }

        internal void LoginCompleted(string formName)
        {
            const string loginForm = "form-login";
            if (!formName.Equals(loginForm, StringComparison.OrdinalIgnoreCase)) return;
            if (!HasWebBrowser.GetValueOrDefault()) return;
            _web.ExecuteScriptAsync(scriptNames[2]);
        }

        private static readonly List<string> scriptNames = new()
            {
                "setIconState",
                "setStatusMessage",
                "loginCompletedAction"
            };
    }
}
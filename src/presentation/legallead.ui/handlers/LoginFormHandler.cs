using legallead.ui.interfaces;

namespace legallead.ui.handlers
{
    internal class LoginFormHandler : IFormHandler
    {
        private const int FormIndex = 0;
        private readonly WebView _web;
        public LoginFormHandler(MainPage main)
        {
            _web = main.WebViewer;
        }


        public void Start()
        {
            scriptNames.ForEach(s =>
            {
                if (scriptNames.IndexOf(s) == 0)
                {
                    // setup animation so user knows process is running
                    var iconCommand = $"{s}( {FormIndex}, true )";
                    _web.EvaluateJavaScriptAsync(iconCommand);
                }
                if (scriptNames.IndexOf(s) == 1)
                {
                    // clear any previous submission messages
                    var statusCommand = $"{s}( {FormIndex}, '', true )";
                    _web.EvaluateJavaScriptAsync(statusCommand);
                }
            });
        }

        public void SetMessage(string htm)
        {
            htm = htm.Replace("'", '"'.ToString());
            var statusCommand = $"{scriptNames[1]}( {FormIndex}, '{htm}', true )";
            _web.EvaluateJavaScriptAsync(statusCommand);
        }

        public void End()
        {
            var iconCommand = $"{scriptNames[0]}( {FormIndex}, false )";
            _web.EvaluateJavaScriptAsync(iconCommand);
        }

        public void SubmissionCompleted()
        {
            _web.EvaluateJavaScriptAsync(scriptNames[2]);
        }

        private static readonly List<string> scriptNames = new()
            {
                "setIconState",
                "setStatusMessage",
                "loginCompletedAction"
            };
    }
}

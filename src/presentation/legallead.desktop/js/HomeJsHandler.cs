using legallead.desktop.handlers;

namespace legallead.desktop.js
{
    internal class HomeJsHandler : JsHandler
    {
        public override string Submit(string formName, string json)
        {
            var response = _handler.Submit(formName, json);
            return response ?? string.Empty;
        }

        private static readonly JsCompletedHandler _handler = new JsHomeFormSubmittedHandler();
    }
}
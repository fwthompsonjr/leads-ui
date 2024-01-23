using legallead.desktop;
using legallead.desktop.entities;
using legallead.desktop.models;
using legallead.desktop.utilities;
using legallead.ui.handlers;
using legallead.ui.interfaces;
using legallead.ui.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace legallead.ui.implementations
{
    internal abstract class PageNavigatorBase
    {
        protected PageNavigatorBase()
        {
            FormHandler = new DefaultFormHandler();
            var main = MainPageFinder.GetMain();
            if (main == null) return;
            mainPage = main;
        }
        protected MainPage? mainPage;
        protected abstract IFormHandler FormHandler { get; set; }

        protected abstract Task<ApiResponse> TryPost(string json);

        protected static T? TryDeserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }

        protected static string ConvertHTML(ApiResponse? response)
        {
            response ??= new ApiResponse { StatusCode = 500, Message = "An unexpected error has occurred" };
            var code = response.StatusCode;
            var message = string.IsNullOrWhiteSpace(response.Message) ? "No data provided" : response.Message;
            return $"{code} | {message}";
        }

        protected static string? GetScriptFromUrl(string? url)
        {
            var keyname = GetContextFromUrl(url);
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrEmpty(keyname)) return null;
            if (!url.Contains(MainPage.InternalDomain)) return null;
            if (!ScriptLib.TryGetValue(keyname, out var scriptjs)) return null;
            return scriptjs;
        }

        public async Task Submit(string url)
        {
            if (mainPage == null) return;
            var jscript = GetScriptFromUrl(url);
            if (string.IsNullOrWhiteSpace(jscript)) return;
            StatusBarHelper.SetStatus(Submitting);
            var web = mainPage.WebViewer;
            var response = await web.EvaluateJavaScriptAsync(jscript);
            if (response == null) return;
            if (BypassLogin(url)) return;
            var handler = FormHandler;
            try
            {
                handler.Start();
                var objectData = TryPost(response).Result;
                var htm = ConvertHTML(objectData);
                handler.SetMessage(htm);
                if (objectData.StatusCode != 200)
                {
                    StatusBarHelper.SetStatus(SubmitFailed);
                    return;
                }

                handler.SubmissionCompleted();
                handler.SetMessage("Submission completed");
                _ = TryDeserialize<LoginFormModel>(response);
                StatusBarHelper.SetStatus(SubmitSucceeded);
            }
            catch (Exception ex)
            {
                StatusBarHelper.SetStatus(SubmitError);
                var exresponse = new ApiResponse { StatusCode = 500, Message = ex.Message };
                var exmsg = ConvertHTML(exresponse);
                handler.SetMessage(exmsg);
            }
            finally
            {
                handler.End();
            }

        }

        private static bool BypassLogin(string url)
        {
            const string allowBypassKey = "debug.login:passthru";
            var context = GetContextFromUrl(url);
            if (string.IsNullOrWhiteSpace(context) || !context.Equals(ScriptKeyNames()[0])) return false;
            var config = AppBuilder.ServiceProvider?.GetService<IConfiguration>();
            if (config == null) return false;
            var kv = config[allowBypassKey];
            if (kv == null || !bool.TryParse(kv, out var allowed)) return false;
            if (!allowed) return false;
            // set status to OK, set user to authenticated
            UserAuthenicationHelper.AuthenicationCompleted(Guid.NewGuid().ToString());
            StatusBarHelper.SetStatus(SubmitSucceeded);
            return true;
        }

        private static string? GetContextFromUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            if (!url.Contains(MainPage.InternalDomain)) return null;
            var components = url.Split('/');
            if (components.Length < 5) return null;
            return $"{components[3]}-{components[4]}";
        }

        protected const int SubmitError = (int)CommonStatusTypes.Error;
        protected const int SubmitFailed = (int)CommonStatusTypes.SubmitFailed;
        protected const int SubmitSucceeded = (int)CommonStatusTypes.SubmitSucceeded;
        protected const int Submitting = (int)CommonStatusTypes.Submitting;

        private static string[] ScriptKeyNames() => ScriptLib.Keys.ToArray();
        protected static readonly Dictionary<string, string> ScriptLib = new()
        {
            { "home-form-login-submit", "serializeFormToObject(0);" },
            { "home-form-register-submit", "serializeFormToObject(1);" }
        };
    }
}
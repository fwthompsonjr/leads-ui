using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using legallead.desktop.utilities;
using legallead.ui.handlers;
using legallead.ui.interfaces;
using legallead.ui.Utilities;
using Newtonsoft.Json;
using System.Text;

namespace legallead.ui.implementations
{
    internal class PageNavigatorHome : IPageNavigator
    {
        private readonly MainPage? mainPage;
        public PageNavigatorHome()
        {

            var main = MainPageFinder.GetMain();
            if (main == null) return;
            mainPage = main;
        }

        public async Task Submit(string url)
        {
            if (mainPage == null || string.IsNullOrEmpty(url)) return;
            if (!url.Contains(MainPage.InternalDomain)) return;
            var components = url.Split('/');
            var componentName = $"{components[3]}-{components[4]}";
            if (!ScriptLib.TryGetValue(componentName, out var scriptjs)) return;
            var web = mainPage.WebViewer;
            var response = await web.EvaluateJavaScriptAsync(scriptjs);
            if (response == null) return;
            var handler = new LoginFormHandler(mainPage, response);
            try
            {
                handler.Start();
                var objectData = TryPost(response).Result;
                var htm = ConvertHTML(objectData);
                handler.SetMessage(htm);
                if (objectData.StatusCode != 200) return;
                handler.LoginCompleted();
                handler.SetMessage("Login completed");
                _ = TryDeserialize<LoginFormModel>(response);
            }
            catch (Exception ex)
            {
                var exresponse = new ApiResponse { StatusCode = 500, Message = ex.Message };
                var exmsg = ConvertHTML(exresponse);
                handler.SetMessage(exmsg);
            }
            finally
            {
                handler.End();
            }

        }


        private static async Task<ApiResponse> TryPost(string json)
        {
            const string failureMessage = "Unable to parse form submission data.";
            var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };

            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return failed;

            var api = provider.GetRequiredService<IPermissionApi>();
            if (api == null) return failed;

            var user = provider.GetRequiredService<UserBo>();
            if (user == null || !user.IsInitialized) return failed;

            var data = TryDeserialize<LoginFormModel>(json);
            if (data == null) return failed;
            var obj = new { data.UserName, data.Password };
            var loginResponse = await api.Post("login", obj, user) ?? failed;
            return loginResponse;

        }

        internal static string ConvertHTML(ApiResponse? response)
        {
            const int statusOk = 200;
            response ??= new ApiResponse { StatusCode = 500, Message = "An unexpected error has occurred" };
            var code = response.StatusCode;
            var statusClass = code == statusOk ? "text-success" : "text-danger";
            var borderCode = code == statusOk ? "border-secordary" : "border-danger";
            var builder = new StringBuilder($"<div class='border {borderCode} m-2 p-2 fs-5cd'>");
            var message = string.IsNullOrWhiteSpace(response.Message) ? "No data provided" : response.Message;
            builder.AppendLine();
            builder.AppendLine($"\t<span name='status-code' class='{statusClass}'> {code:D3} Status </span> <br/>");
            builder.AppendLine($"\t<span name='status-message' class='text-secondary'> {message} </span> <br/>");
            builder.AppendLine("</div>");
            return builder.ToString();
        }

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

        private static readonly Dictionary<string, string> ScriptLib = new()
        {
            { "home-form-login-submit", "serializeFormToObject(0);" },
            { "home-form-register-submit", "serializeFormToObject(1);" }
        };
    }
}

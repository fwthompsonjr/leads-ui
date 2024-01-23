using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using legallead.ui.handlers;
using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class PageNavigatorRegistration : PageNavigatorBase, IPageNavigator
    {
        public PageNavigatorRegistration() : base()
        {
            FormHandler ??= new DefaultFormHandler();
            if (mainPage != null)
                FormHandler = new RegistrationFormHandler(mainPage);
        }

        protected override IFormHandler FormHandler { get; set; }

        protected override async Task<ApiResponse> TryPost(string json)
        {
            const string failureMessage = "Unable to parse form submission data.";
            var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };

            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return failed;

            var api = provider.GetRequiredService<IPermissionApi>();
            if (api == null) return failed;

            var user = provider.GetRequiredService<UserBo>();
            if (user == null || !user.IsInitialized) return failed;

            var data = TryDeserialize<UserRegistrationModel>(json);
            if (data == null) return failed;
            var obj = data.ToApiModel();

            var registerResponse = await api.Post("register", obj, user) ?? failed;
            return registerResponse;

        }
    }
}
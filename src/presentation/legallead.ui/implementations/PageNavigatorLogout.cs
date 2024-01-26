using legallead.desktop.entities;
using legallead.ui.handlers;
using legallead.ui.interfaces;
using legallead.ui.Utilities;

namespace legallead.ui.implementations
{
    internal class PageNavigatorLogout : PageNavigatorBase, IPageNavigator
    {
        public PageNavigatorLogout() : base()
        {
            FormHandler ??= new DefaultFormHandler();
        }

        protected override IFormHandler FormHandler { get; set; }

        public override Task Submit(string url)
        {
            UserAuthenicationHelper.LogoutRequested();
            var handler = MainPageFinder.GetMain()?.HomeHandler;
            handler?.SetHome();
            return Task.CompletedTask;
        }
        protected override async Task<ApiResponse> TryPost(string json)
        {
            var response = await Task.Run(() =>
            {

                const string message = "User logout completed.";
                return new ApiResponse { StatusCode = 200, Message = message };
            });
            return response;
        }
    }
}
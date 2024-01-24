namespace legallead.ui.Utilities
{
    internal class MyAccountContentLoadHandler() : ContentLoadBase
    {
        public void SetHome()
        {
            var homepage = GetHTML("myaccount");
            SetView(homepage);
        }
    }
}

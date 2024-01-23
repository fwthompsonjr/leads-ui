namespace legallead.ui.Utilities
{
    internal class MyAccountContentLoadHandler() : ContentLoadBase
    {
        public void SetHome()
        {
            var homepage = ButtonClickWriter.ReWrite("myaccount");
            SetView(homepage);
        }
    }
}

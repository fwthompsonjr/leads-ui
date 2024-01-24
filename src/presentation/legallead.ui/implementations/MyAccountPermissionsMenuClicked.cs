using HtmlAgilityPack;
using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class MyAccountPermissionsMenuClicked : MyAccountClickBase, IMenuClickHandler
    {
        protected override string PageTarget => "myaccount";
        protected virtual int PageId => 2;
        protected override string Transform(string text)
        {
            const char space = ' ';
            const string actv = "active";
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            for (int i = 0; i < menus.Length; i++)
            {
                var menu = doc.DocumentNode.SelectSingleNode(menus[i]);
                var dv = doc.DocumentNode.SelectSingleNode(divs[i]);
                if (menu == null || dv == null) continue;

                var attrmenu = menu.Attributes.ToList().Find(x => x.Name.Equals("class"));
                var attrdiv = dv.Attributes.ToList().Find(x => x.Name.Equals("class"));
                if (attrmenu == null || attrdiv == null) continue;

                var clsmenu = attrmenu.Value.Split(space).ToList();
                var clsdiv = attrdiv.Value.Split(space).ToList();
                if (i == PageId)
                {
                    if (!clsmenu.Exists(x => x.Equals(actv))) clsmenu.Add(actv);
                    if (!clsdiv.Exists(x => x.Equals(actv))) clsdiv.Add(actv);
                }
                else
                {
                    clsmenu.RemoveAll(x => x.Equals(actv));
                    clsdiv.RemoveAll(x => x.Equals(actv));
                }
                attrmenu.Value = string.Join(" ", clsmenu);
                attrdiv.Value = string.Join(" ", clsdiv);

            }
            return doc.DocumentNode.OuterHtml;
        }

        protected static readonly string[] menus = ["//a[@name='subcontent-home']", "//a[@name='subcontent-profile']", "//a[@name='subcontent-permissions']"];
        protected static readonly string[] divs = ["//*[@id='dv-subcontent-home']", "//*[@id='dv-subcontent-profile']", "//*[@id='dv-subcontent-permissions']"];
    }
}
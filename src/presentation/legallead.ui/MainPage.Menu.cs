using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.ui
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
    {

        private void InitializeMenus()
        {

        }

        void OnItemClicked(object sender, EventArgs e)
        {
            if (sender is not MenuItem menuItem) return;

            // Access the list item through the BindingContext
            var contextItem = menuItem.BindingContext;

            // Do something with the contextItem here
        }
    }
}

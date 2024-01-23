namespace legallead.ui.Controls
{
    internal class BindableToolbarItem : MenuBarItem
    {
        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(BindableToolbarItem), true, BindingMode.OneWay, propertyChanged: OnIsVisibleChanged);

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        protected override void OnParentChanged()
        {
            base.OnParentChanged();

            RefreshVisibility();
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is not BindableToolbarItem item) return;
            item.RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            if (Parent == null)
            {
                return;
            }

            bool value = IsVisible;
            if (Parent is not MainPage main) return;
            var items = main.MenuBarItems;
            if (value && !items.Contains(this))
            {
                Application.Current?.Dispatcher.Dispatch(() => { items.Add(this); });
            }
            else if (!value && items.Contains(this))
            {
                Application.Current?.Dispatcher.Dispatch(() => { items.Remove(this); });
            }
        }
    }
}

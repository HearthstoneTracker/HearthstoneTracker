namespace HearthCap.UI.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class SelectingItemAttachedProperty
    {
        public static readonly DependencyProperty SelectingItemProperty = DependencyProperty.RegisterAttached(
            "SelectingItem",
            typeof(object),
            typeof(SelectingItemAttachedProperty),
            new PropertyMetadata(default(object), OnSelectingItemChanged));

        public static object GetSelectingItem(DependencyObject target)
        {
            return target.GetValue(SelectingItemProperty);
        }

        public static void SetSelectingItem(DependencyObject target, object value)
        {
            target.SetValue(SelectingItemProperty, value);
        }

        static void OnSelectingItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid != null && grid.SelectedItem != null)
            {
                grid.Dispatcher.InvokeAsync(() =>
                {
                    if (grid.SelectedItem == null)
                        return;

                    grid.UpdateLayout();
                    grid.ScrollIntoView(grid.SelectedItem, null);
                });                
            }

            var lv = sender as ListView;
            if (lv != null && lv.SelectedItem != null)
            {
                lv.Dispatcher.InvokeAsync(() =>
                {
                    if (lv.SelectedItem == null)
                        return;

                    lv.UpdateLayout();
                    lv.ScrollIntoView(lv.SelectedItem);
                });
            }
        }
    }
}
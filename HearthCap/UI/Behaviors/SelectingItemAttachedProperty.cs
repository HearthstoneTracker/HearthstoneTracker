// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectingItemAttachedProperty.cs" company="">
//   
// </copyright>
// <summary>
//   The selecting item attached property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The selecting item attached property.
    /// </summary>
    public class SelectingItemAttachedProperty
    {
        /// <summary>
        /// The selecting item property.
        /// </summary>
        public static readonly DependencyProperty SelectingItemProperty = DependencyProperty.RegisterAttached(
            "SelectingItem", 
            typeof(object), 
            typeof(SelectingItemAttachedProperty), 
            new PropertyMetadata(default(object), OnSelectingItemChanged));

        /// <summary>
        /// The get selecting item.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetSelectingItem(DependencyObject target)
        {
            return target.GetValue(SelectingItemProperty);
        }

        /// <summary>
        /// The set selecting item.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetSelectingItem(DependencyObject target, object value)
        {
            target.SetValue(SelectingItemProperty, value);
        }

        /// <summary>
        /// The on selecting item changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListBoxBehaviour.cs" company="">
//   
// </copyright>
// <summary>
//   The list box behaviour.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The list box behaviour.
    /// </summary>
    public static class ListBoxBehaviour
    {
        /// <summary>
        /// The auto copy property.
        /// </summary>
        public static readonly DependencyProperty AutoCopyProperty = DependencyProperty.RegisterAttached("AutoCopy", 
            typeof(bool), typeof(ListBoxBehaviour), new UIPropertyMetadata(AutoCopyChanged));

        /// <summary>
        /// The get auto copy.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetAutoCopy(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoCopyProperty);
        }

        /// <summary>
        /// The set auto copy.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetAutoCopy(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoCopyProperty, value);
        }

        /// <summary>
        /// The auto copy changed.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="e_">
        /// The e_.
        /// </param>
        private static void AutoCopyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e_)
        {
            var listBox = obj as ListBox;
            if (listBox != null)
            {
                if ((bool)e_.NewValue)
                {
                    ExecutedRoutedEventHandler handler =
                        (sender, arg) =>
                        {
                            if (listBox.SelectedItems.Count > 0)
                            {
                                var sb = new StringBuilder();
                                foreach (var selectedItem in listBox.SelectedItems)
                                {
                                    sb.AppendLine(selectedItem.ToString());
                                }

                                Clipboard.SetDataObject(sb.ToString());
                            }
                        };

                    var command = new RoutedCommand("Copy", typeof(ListBox));
                    command.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control, "Copy"));
                    listBox.CommandBindings.Add(new CommandBinding(command, handler));
                }
            }
        }
    }
}
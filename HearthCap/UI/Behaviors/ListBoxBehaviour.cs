namespace HearthCap.UI.Behaviors
{
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class ListBoxBehaviour
    {
        public static readonly DependencyProperty AutoCopyProperty = DependencyProperty.RegisterAttached("AutoCopy",
            typeof(bool), typeof(ListBoxBehaviour), new UIPropertyMetadata(AutoCopyChanged));

        public static bool GetAutoCopy(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoCopyProperty);
        }

        public static void SetAutoCopy(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoCopyProperty, value);
        }

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
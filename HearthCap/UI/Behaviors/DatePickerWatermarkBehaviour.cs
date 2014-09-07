namespace HearthCap.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public static class DatePickerWatermarkBehaviour
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached(
                "Watermark",
                typeof(string),
                typeof(DatePickerWatermarkBehaviour),
                new UIPropertyMetadata(null, OnWatermarkChanged));

        public static string GetWatermark(Control control)
        {
            return (string)control.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(Control control, string value)
        {
            control.SetValue(WatermarkProperty, value);
        }

        private static void OnWatermarkChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = dependencyObject as DatePicker;
            if (datePicker == null)
                return;

            if ((e.NewValue != null) && (e.OldValue == null))
                datePicker.Loaded += DatePickerLoaded;
            else if ((e.NewValue == null) && (e.OldValue != null))
                datePicker.Loaded -= DatePickerLoaded;
        }

        private static void DatePickerLoaded(object sender, RoutedEventArgs e)
        {
            var datePicker = sender as DatePicker;
            if (datePicker == null)
                return;

            var datePickerTextBox = GetFirstChildOfType<DatePickerTextBox>(datePicker);
            if (datePickerTextBox == null)
                return;

            var partWatermark = datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox) as ContentControl;
            if (partWatermark == null)
                return;

            partWatermark.Content = GetWatermark(datePicker);
        }

        private static T GetFirstChildOfType<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject == null)
                return null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                var result = (child as T) ?? GetFirstChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
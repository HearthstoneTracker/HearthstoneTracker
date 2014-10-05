﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatePickerWatermarkBehaviour.cs" company="">
//   
// </copyright>
// <summary>
//   The date picker watermark behaviour.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    /// <summary>
    /// The date picker watermark behaviour.
    /// </summary>
    public static class DatePickerWatermarkBehaviour
    {
        /// <summary>
        /// The watermark property.
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached(
                "Watermark", 
                typeof(string), 
                typeof(DatePickerWatermarkBehaviour), 
                new UIPropertyMetadata(null, OnWatermarkChanged));

        /// <summary>
        /// The get watermark.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetWatermark(Control control)
        {
            return (string)control.GetValue(WatermarkProperty);
        }

        /// <summary>
        /// The set watermark.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetWatermark(Control control, string value)
        {
            control.SetValue(WatermarkProperty, value);
        }

        /// <summary>
        /// The on watermark changed.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

        /// <summary>
        /// The date picker loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

        /// <summary>
        /// The get first child of type.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
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
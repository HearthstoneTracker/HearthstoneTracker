namespace HearthCap.UI.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    [System.Windows.Markup.MarkupExtensionReturnType(typeof(IValueConverter))]
    public sealed class VisbilityToBooleanConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension. 
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new VisbilityToBooleanConverter();
        }
    }
}
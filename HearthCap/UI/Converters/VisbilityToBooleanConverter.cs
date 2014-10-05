// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisbilityToBooleanConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The visbility to boolean converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// The visbility to boolean converter.
    /// </summary>
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public sealed class VisbilityToBooleanConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension. 
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">
        /// A service provider helper that can provide services for the markup extension.
        /// </param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new VisbilityToBooleanConverter();
        }
    }
}
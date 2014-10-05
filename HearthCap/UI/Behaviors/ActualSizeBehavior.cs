// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActualSizeBehavior.cs" company="">
//   
// </copyright>
// <summary>
//   The actual size behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System.Windows;

    /// <summary>
    /// The actual size behavior.
    /// </summary>
    public static class ActualSizeBehavior
    {
        /// <summary>
        /// The actual size property.
        /// </summary>
        public static readonly DependencyProperty ActualSizeProperty =
            DependencyProperty.RegisterAttached("ActualSize", 
                                                typeof(bool), 
                                                typeof(ActualSizeBehavior), 
                                                new UIPropertyMetadata(false, OnActualSizeChanged));

        /// <summary>
        /// The get actual size.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetActualSize(DependencyObject obj)
        {
            return (bool)obj.GetValue(ActualSizeProperty);
        }

        /// <summary>
        /// The set actual size.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetActualSize(DependencyObject obj, bool value)
        {
            obj.SetValue(ActualSizeProperty, value);
        }

        /// <summary>
        /// The on actual size changed.
        /// </summary>
        /// <param name="dpo">
        /// The dpo.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnActualSizeChanged(DependencyObject dpo, 
                                                DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = dpo as FrameworkElement;
            if ((bool)e.NewValue)
            {
                element.SizeChanged += element_SizeChanged;
            }
            else
            {
                element.SizeChanged -= element_SizeChanged;
            }
        }

        /// <summary>
        /// The element_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        static void element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            SetActualWidth(element, element.ActualWidth);
            SetActualHeight(element, element.ActualHeight);
        }

        /// <summary>
        /// The actual width property.
        /// </summary>
        private static readonly DependencyProperty ActualWidthProperty =
            DependencyProperty.RegisterAttached("ActualWidth", typeof(double), typeof(ActualSizeBehavior));

        /// <summary>
        /// The set actual width.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetActualWidth(DependencyObject element, double value)
        {
            element.SetValue(ActualWidthProperty, value);
        }

        /// <summary>
        /// The get actual width.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double GetActualWidth(DependencyObject element)
        {
            return (double)element.GetValue(ActualWidthProperty);
        }

        /// <summary>
        /// The actual height property.
        /// </summary>
        private static readonly DependencyProperty ActualHeightProperty =
            DependencyProperty.RegisterAttached("ActualHeight", typeof(double), typeof(ActualSizeBehavior));

        /// <summary>
        /// The set actual height.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetActualHeight(DependencyObject element, double value)
        {
            element.SetValue(ActualHeightProperty, value);
        }

        /// <summary>
        /// The get actual height.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double GetActualHeight(DependencyObject element)
        {
            return (double)element.GetValue(ActualHeightProperty);
        }
    }
}
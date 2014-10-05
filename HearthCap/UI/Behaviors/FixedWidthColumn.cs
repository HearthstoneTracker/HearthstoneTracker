// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedWidthColumn.cs" company="">
//   
// </copyright>
// <summary>
//   The fixed width column.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HearthCap.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The fixed width column.
    /// </summary>
    public class FixedWidthColumn : GridViewColumn
    {
        /// <summary>
        /// Initializes static members of the <see cref="FixedWidthColumn"/> class.
        /// </summary>
        static FixedWidthColumn()
        {
            WidthProperty.OverrideMetadata(typeof(FixedWidthColumn), 
                new FrameworkPropertyMetadata(null, OnCoerceWidth));
        }

        /// <summary>
        /// Gets or sets the fixed width.
        /// </summary>
        public double FixedWidth
        {
            get { return (double)this.GetValue(FixedWidthProperty); }
            set { this.SetValue(FixedWidthProperty, value); }
        }

        /// <summary>
        /// The fixed width property.
        /// </summary>
        public static readonly DependencyProperty FixedWidthProperty =
            DependencyProperty.Register(
                "FixedWidth", 
                typeof(double), 
                typeof(FixedWidthColumn), 
                new FrameworkPropertyMetadata(double.NaN, OnFixedWidthChanged));

        /// <summary>
        /// The on fixed width changed.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnFixedWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FixedWidthColumn fwc = o as FixedWidthColumn;
            if (fwc != null)
                fwc.CoerceValue(WidthProperty);
        }

        /// <summary>
        /// The on coerce width.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="baseValue">
        /// The base value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object OnCoerceWidth(DependencyObject o, object baseValue)
        {
            FixedWidthColumn fwc = o as FixedWidthColumn;
            if (fwc != null)
                return fwc.FixedWidth;
            return baseValue;
        }
    }
}
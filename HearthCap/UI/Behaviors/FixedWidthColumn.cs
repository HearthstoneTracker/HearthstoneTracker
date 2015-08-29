using System.Windows;
using System.Windows.Controls;

namespace HearthCap.UI.Behaviors
{
    public class FixedWidthColumn : GridViewColumn
    {
        static FixedWidthColumn()
        {
            WidthProperty.OverrideMetadata(typeof(FixedWidthColumn),
                new FrameworkPropertyMetadata(null, OnCoerceWidth));
        }

        public double FixedWidth
        {
            get { return (double)GetValue(FixedWidthProperty); }
            set { SetValue(FixedWidthProperty, value); }
        }

        public static readonly DependencyProperty FixedWidthProperty =
            DependencyProperty.Register(
                "FixedWidth",
                typeof(double),
                typeof(FixedWidthColumn),
                new FrameworkPropertyMetadata(double.NaN, OnFixedWidthChanged));

        private static void OnFixedWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var fwc = o as FixedWidthColumn;
            if (fwc != null)
            {
                fwc.CoerceValue(WidthProperty);
            }
        }

        private static object OnCoerceWidth(DependencyObject o, object baseValue)
        {
            var fwc = o as FixedWidthColumn;
            if (fwc != null)
            {
                return fwc.FixedWidth;
            }
            return baseValue;
        }
    }
}

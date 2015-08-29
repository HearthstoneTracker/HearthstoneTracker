using System.Windows;

namespace HearthCap.UI.Behaviors
{
    public static class ActualSizeBehavior
    {
        public static readonly DependencyProperty ActualSizeProperty =
            DependencyProperty.RegisterAttached("ActualSize",
                typeof(bool),
                typeof(ActualSizeBehavior),
                new UIPropertyMetadata(false, OnActualSizeChanged));

        public static bool GetActualSize(DependencyObject obj)
        {
            return (bool)obj.GetValue(ActualSizeProperty);
        }

        public static void SetActualSize(DependencyObject obj, bool value)
        {
            obj.SetValue(ActualSizeProperty, value);
        }

        private static void OnActualSizeChanged(DependencyObject dpo,
            DependencyPropertyChangedEventArgs e)
        {
            var element = dpo as FrameworkElement;
            if ((bool)e.NewValue)
            {
                element.SizeChanged += element_SizeChanged;
            }
            else
            {
                element.SizeChanged -= element_SizeChanged;
            }
        }

        private static void element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            SetActualWidth(element, element.ActualWidth);
            SetActualHeight(element, element.ActualHeight);
        }

        private static readonly DependencyProperty ActualWidthProperty =
            DependencyProperty.RegisterAttached("ActualWidth", typeof(double), typeof(ActualSizeBehavior));

        public static void SetActualWidth(DependencyObject element, double value)
        {
            element.SetValue(ActualWidthProperty, value);
        }

        public static double GetActualWidth(DependencyObject element)
        {
            return (double)element.GetValue(ActualWidthProperty);
        }

        private static readonly DependencyProperty ActualHeightProperty =
            DependencyProperty.RegisterAttached("ActualHeight", typeof(double), typeof(ActualSizeBehavior));

        public static void SetActualHeight(DependencyObject element, double value)
        {
            element.SetValue(ActualHeightProperty, value);
        }

        public static double GetActualHeight(DependencyObject element)
        {
            return (double)element.GetValue(ActualHeightProperty);
        }
    }
}

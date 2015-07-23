namespace HearthCap.UI.Behaviors
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    public static class BubbleScrollEventManager
    {
        public static readonly DependencyProperty BubbleScrollEvent = DependencyProperty.RegisterAttached(
            "BubbleScrollEvent",
            typeof(bool),
            typeof(BubbleScrollEventManager),
            new PropertyMetadata(false, OnBubbleScrollEventChanged));

        private static void OnBubbleScrollEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var el = d as FrameworkElement;
            if (el == null) return;

            var bubble = GetBubbleScrollEvent(d);
            var behaviors = Interaction.GetBehaviors(d);
            var bubbleBehavior = behaviors.FirstOrDefault(x => x is BubbleScrollEvent);
            if (bubbleBehavior == null && bubble)
            {
                behaviors.Add(new BubbleScrollEvent());
            }
            else if (bubbleBehavior != null && !bubble)
            {
                behaviors.Remove(bubbleBehavior);
            }
        }

        public static bool GetBubbleScrollEvent(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(BubbleScrollEvent);
        }

        public static void SetBubbleScrollEvent(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(BubbleScrollEvent, value);
        }
    }

    public sealed class BubbleScrollEvent : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            AssociatedObject.RaiseEvent(e2);
        }
    }
}
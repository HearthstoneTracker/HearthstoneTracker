// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BubbleScrollEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The bubble scroll event manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// The bubble scroll event manager.
    /// </summary>
    public static class BubbleScrollEventManager
    {
        /// <summary>
        /// The bubble scroll event.
        /// </summary>
        public static readonly DependencyProperty BubbleScrollEvent = DependencyProperty.RegisterAttached(
            "BubbleScrollEvent", 
            typeof(bool), 
            typeof(BubbleScrollEventManager), 
            new PropertyMetadata(false, OnBubbleScrollEventChanged));

        /// <summary>
        /// The on bubble scroll event changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

        /// <summary>
        /// The get bubble scroll event.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetBubbleScrollEvent(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(BubbleScrollEvent);
        }

        /// <summary>
        /// The set bubble scroll event.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetBubbleScrollEvent(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(BubbleScrollEvent, value);
        }
    }

    /// <summary>
    /// The bubble scroll event.
    /// </summary>
    public sealed class BubbleScrollEvent : Behavior<UIElement>
    {
        /// <summary>
        /// The on attached.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
        }

        /// <summary>
        /// The on detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        /// <summary>
        /// The associated object_ preview mouse wheel.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            this.AssociatedObject.RaiseEvent(e2);
        }
    }
}
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace HearthCap.UI.Behaviors
{
    public class RoutedEventTrigger : EventTriggerBase<DependencyObject>
    {
        public RoutedEvent RoutedEvent { get; set; }

        protected override void OnAttached()
        {
            var behavior = AssociatedObject as Behavior;
            var associatedElement = AssociatedObject as FrameworkElement;

            if (behavior != null)
            {
                associatedElement = ((IAttachedObject)behavior).AssociatedObject as FrameworkElement;
            }
            if (associatedElement == null)
            {
                throw new ArgumentException("Routed Event trigger can only be associated to framework elements");
            }
            if (RoutedEvent != null)
            {
                associatedElement.AddHandler(RoutedEvent, new RoutedEventHandler(OnRoutedEvent));
            }
        }

        private void OnRoutedEvent(object sender, RoutedEventArgs args)
        {
            OnEvent(args);
        }

        protected override string GetEventName()
        {
            return RoutedEvent.Name;
        }
    }
}

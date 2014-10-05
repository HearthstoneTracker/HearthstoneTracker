// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutedEventTrigger.cs" company="">
//   
// </copyright>
// <summary>
//   The routed event trigger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Interactivity;

    /// <summary>
    /// The routed event trigger.
    /// </summary>
    public class RoutedEventTrigger : EventTriggerBase<DependencyObject>
    {
        /// <summary>
        /// The _routed event.
        /// </summary>
        RoutedEvent _routedEvent;

        /// <summary>
        /// Gets or sets the routed event.
        /// </summary>
        public RoutedEvent RoutedEvent
        {
            get { return this._routedEvent; }
            set { this._routedEvent = value; }
        }

        /// <summary>
        /// The on attached.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// </exception>
        protected override void OnAttached()
        {
            Behavior behavior = this.AssociatedObject as Behavior;
            FrameworkElement associatedElement = this.AssociatedObject as FrameworkElement;

            if (behavior != null)
            {
                associatedElement = ((IAttachedObject)behavior).AssociatedObject as FrameworkElement;
            }

            if (associatedElement == null)
            {
                throw new ArgumentException("Routed Event trigger can only be associated to framework elements");
            }

            if (this.RoutedEvent != null)
            {
                associatedElement.AddHandler(this.RoutedEvent, new RoutedEventHandler(this.OnRoutedEvent));
            }
        }

        /// <summary>
        /// The on routed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        void OnRoutedEvent(object sender, RoutedEventArgs args)
        {
            this.OnEvent(args);
        }

        /// <summary>
        /// The get event name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetEventName()
        {
            return this.RoutedEvent.Name;
        }
    }
}
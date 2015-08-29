///***NOTE - From:
/// http://blogs.msdn.com/b/delay/archive/2009/03/09/controls-are-like-diapers-you-don-t-want-a-leaky-one-implementing-the-weakevent-pattern-on-silverlight-with-the-weakeventlistener-class.aspx
/// Used with Permission
/// w/ updates from:
/// http://blog.thekieners.com/2010/02/11/simple-weak-event-listener-for-silverlight/

using System;

namespace HearthCap.UI.Behaviors.DragDrop
{
    /// <summary>
    ///     Implements a weak event listener that allows the owner to be garbage
    ///     collected if its only remaining link is an event handler.
    /// </summary>
    /// <typeparam name="TInstance">Type of instance listening for the event.</typeparam>
    /// <typeparam name="TSource">Type of source for the event.</typeparam>
    /// <typeparam name="TEventArgs">Type of event arguments for the event.</typeparam>
    internal class WeakEventListener<TInstance, TSource, TEventArgs>
        where TInstance : class
    {
        /// <summary>
        ///     WeakReference to the instance listening for the event.
        /// </summary>
        private readonly WeakReference _weakInstance;

        private readonly WeakReference _weakSource;

        /// <summary>
        ///     Gets or sets the method to call when the event fires.
        /// </summary>
        public Action<TInstance, object, TEventArgs> OnEventAction { get; set; }

        /// <summary>
        ///     Gets or sets the method to call when detaching from the event.
        /// </summary>
        public Action<WeakEventListener<TInstance, TSource, TEventArgs>, TSource> OnDetachAction { get; set; }

        /// <summary>
        ///     Initializes a new instances of the WeakEventListener class.
        /// </summary>
        /// <param name="instance">Instance subscribing to the event.</param>
        public WeakEventListener(TInstance instance, TSource source)
        {
            if (null == instance)
            {
                throw new ArgumentNullException("instance");
            }
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }

            _weakInstance = new WeakReference(instance);
            _weakSource = new WeakReference(source);
        }

        /// <summary>
        ///     Handler for the subscribed event calls OnEventAction to handle it.
        /// </summary>
        /// <param name="source">Event source.</param>
        /// <param name="eventArgs">Event arguments.</param>
        public void OnEvent(object source, TEventArgs eventArgs)
        {
            var target = (TInstance)_weakInstance.Target;
            if (null != target)
            {
                // Call registered action
                if (null != OnEventAction)
                {
                    OnEventAction(target, source, eventArgs);
                }
            }
            else
            {
                // Detach from event
                Detach();
            }
        }

        /// <summary>
        ///     Detaches from the subscribed event.
        /// </summary>
        public void Detach()
        {
            var source = (TSource)_weakSource.Target;
            if (null != OnDetachAction
                && null != source)
            {
                OnDetachAction(this, source);
                OnDetachAction = null;
            }
        }
    }
}

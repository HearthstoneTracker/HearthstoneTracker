// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="WeakEventListener.cs">
//   
// </copyright>
// <summary>
//   Implements a weak event listener that allows the owner to be garbage
//   collected if its only remaining link is an event handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HearthCap.UI.Behaviors.DragDrop
{
    using System;

    /// <summary>
    /// Implements a weak event listener that allows the owner to be garbage
    /// collected if its only remaining link is an event handler.
    /// </summary>
    /// <typeparam name="TInstance">
    /// Type of instance listening for the event.
    /// </typeparam>
    /// <typeparam name="TSource">
    /// Type of source for the event.
    /// </typeparam>
    /// <typeparam name="TEventArgs">
    /// Type of event arguments for the event.
    /// </typeparam>
    internal class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
    {
        /// <summary>
        /// WeakReference to the instance listening for the event.
        /// </summary>
        private WeakReference _weakInstance;

        /// <summary>
        /// The _weak source.
        /// </summary>
        private WeakReference _weakSource;

        /// <summary>
        /// Gets or sets the method to call when the event fires.
        /// </summary>
        public Action<TInstance, object, TEventArgs> OnEventAction { get; set; }

        /// <summary>
        /// Gets or sets the method to call when detaching from the event.
        /// </summary>
        public Action<WeakEventListener<TInstance, TSource, TEventArgs>, TSource> OnDetachAction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventListener{TInstance,TSource,TEventArgs}"/> class. 
        /// Initializes a new instances of the WeakEventListener class.
        /// </summary>
        /// <param name="instance">
        /// Instance subscribing to the event.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
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

            this._weakInstance = new WeakReference(instance);
            this._weakSource = new WeakReference(source);
        }

        /// <summary>
        /// Handler for the subscribed event calls OnEventAction to handle it.
        /// </summary>
        /// <param name="source">
        /// Event source.
        /// </param>
        /// <param name="eventArgs">
        /// Event arguments.
        /// </param>
        public void OnEvent(object source, TEventArgs eventArgs)
        {
            TInstance target = (TInstance)this._weakInstance.Target;
            if (null != target)
            {
                // Call registered action
                if (null != this.OnEventAction)
                {
                    this.OnEventAction(target, source, eventArgs);
                }
            }
            else
            {
                // Detach from event
                this.Detach();
            }
        }

        /// <summary>
        /// Detaches from the subscribed event.
        /// </summary>
        public void Detach()
        {
            TSource source = (TSource)this._weakSource.Target;
            if (null != this.OnDetachAction && null != source)
            {
                this.OnDetachAction(this, source);
                this.OnDetachAction = null;
            }
        }
    }
}

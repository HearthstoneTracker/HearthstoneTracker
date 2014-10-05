// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGridDragDropEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The data grid drag drop event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors.DragDrop
{
    using System;
    using System.Windows;

    /// <summary>
    /// The data grid drag drop event args.
    /// </summary>
    public class DataGridDragDropEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the source.
        /// </summary>
        public object Source { get; internal set; }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        public object Destination { get; internal set; }

        /// <summary>
        /// Gets the dropped object.
        /// </summary>
        public object DroppedObject { get; internal set; }

        /// <summary>
        /// Gets the target object.
        /// </summary>
        public object TargetObject { get; internal set; }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public DataGridDragDropDirection Direction { get; internal set; }

        /// <summary>
        /// Gets the effects.
        /// </summary>
        public DragDropEffects Effects { get; internal set; }
    }
}

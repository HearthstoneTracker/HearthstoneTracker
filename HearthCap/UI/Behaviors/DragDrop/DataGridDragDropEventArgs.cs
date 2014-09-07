namespace HearthCap.UI.Behaviors.DragDrop
{
    using System;
    using System.Windows;

    public class DataGridDragDropEventArgs : EventArgs
    {
        public object Source { get; internal set; }
        public object Destination { get; internal set; }

        public object DroppedObject { get; internal set; }
        public object TargetObject { get; internal set; }

        public DataGridDragDropDirection Direction { get; internal set; }
        public DragDropEffects Effects { get; internal set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGridDragDropBehavior.cs" company="">
//   
// </copyright>
// <summary>
//   The relay command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors.DragDrop
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// The relay command.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        /// <summary>
        /// The _execute.
        /// </summary>
        private readonly Action<T> _execute;

        /// <summary>
        /// The _can execute.
        /// </summary>
        private readonly Predicate<T> _canExecute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class. 
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">
        /// The execution logic.
        /// </param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class. 
        /// Creates a new command with conditional execution.
        /// </summary>
        /// <param name="execute">
        /// The execution logic.
        /// </param>
        /// <param name="canExecute">
        /// The execution status logic.
        /// </param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this._execute = execute;
            this._canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute((T)parameter);
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this._canExecute != null)
                    CommandManager.RequerySuggested += value;
            }

            remove
            {
                if (this._canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this._execute((T)parameter);
        }

        #endregion
    }

    /// <summary>
    /// The data grid drag drop behavior.
    /// </summary>
    public class DataGridDragDropBehavior : Behavior<DataGrid>
    {
        #region Dependency Properties

        /// <summary>
        /// The command property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(RelayCommand<DataGridDragDropEventArgs>), typeof(DataGridDragDropBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// The allowed effects property.
        /// </summary>
        public static readonly DependencyProperty AllowedEffectsProperty =
            DependencyProperty.Register("AllowedEffects", typeof(DragDropEffects), typeof(DataGridDragDropBehavior), new UIPropertyMetadata(DragDropEffects.Move));

        /// <summary>
        /// The drop target property.
        /// </summary>
        public static readonly DependencyProperty DropTargetProperty =
            DependencyProperty.Register("DropTarget", typeof(DataGrid), typeof(DataGridDragDropBehavior), new UIPropertyMetadata());

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the allowed effects.
        /// </summary>
        public DragDropEffects AllowedEffects
        {
            get { return (DragDropEffects)this.GetValue(AllowedEffectsProperty); }
            set { this.SetValue(AllowedEffectsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public RelayCommand<DataGridDragDropEventArgs> Command
        {
            get { return (RelayCommand<DataGridDragDropEventArgs>)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the drop target.
        /// </summary>
        public DataGrid DropTarget
        {
            get { return (DataGrid)this.GetValue(DropTargetProperty); }
            set { this.SetValue(DropTargetProperty, value); }
        }
        #endregion

        /// <summary>
        /// The _drop target.
        /// </summary>
        private object _dropTarget;

        /// <summary>
        /// The _direction.
        /// </summary>
        private DataGridDragDropDirection _direction = DataGridDragDropDirection.Indeterminate;

        /// <summary>
        /// The _source.
        /// </summary>
        private object _source;

        /// <summary>
        /// The _destination.
        /// </summary>
        private object _destination;

        /// <summary>
        /// The _last index.
        /// </summary>
        private int _lastIndex = -1;

        /// <summary>
        /// The on attached.
        /// </summary>
        protected override void OnAttached()
        {
            // Mouse Move
            WeakEventListener<DataGridDragDropBehavior, DataGrid, MouseEventArgs> mouseListener = new WeakEventListener<DataGridDragDropBehavior, DataGrid, MouseEventArgs>(this, this.AssociatedObject);
            mouseListener.OnEventAction = (instance, source, args) => instance.DataGrid_MouseMove(source, args);
            mouseListener.OnDetachAction = (listenerRef, source) => source.MouseMove -= listenerRef.OnEvent;
            this.AssociatedObject.MouseMove += mouseListener.OnEvent;

            // Drag Enter/Leave/Over
            WeakEventListener<DataGridDragDropBehavior, DataGrid, DragEventArgs> dragListener = new WeakEventListener<DataGridDragDropBehavior, DataGrid, DragEventArgs>(this, this.AssociatedObject);
            dragListener.OnEventAction = (instance, source, args) => instance.DataGrid_CheckDropTarget(source, args);
            dragListener.OnDetachAction = (listenerRef, source) =>
            {
                source.DragEnter -= listenerRef.OnEvent;
                source.DragLeave -= listenerRef.OnEvent;
                source.DragOver -= listenerRef.OnEvent;
            };
            this.AssociatedObject.DragEnter += dragListener.OnEvent;
            this.AssociatedObject.DragLeave += dragListener.OnEvent;
            this.AssociatedObject.DragOver += dragListener.OnEvent;

            // Drop
            var target = this.DropTarget ?? this.AssociatedObject;
            WeakEventListener<DataGridDragDropBehavior, DataGrid, DragEventArgs> dropListener = new WeakEventListener<DataGridDragDropBehavior, DataGrid, DragEventArgs>(this, target);
            dropListener.OnEventAction = (instance, source, args) => instance.DataGrid_Drop(source, args);
            dropListener.OnDetachAction = (listenerRef, source) => source.Drop -= listenerRef.OnEvent;
            target.Drop += dropListener.OnEvent;

            base.OnAttached();
        }

        /// <summary>
        /// The data grid_ mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataGridRow row = UIHelper.FindVisualParent<DataGridRow>(e.OriginalSource as FrameworkElement);
                if (row != null && row.IsSelected)
                {
                    this._source = UIHelper.FindVisualParent<DataGrid>(row).ItemsSource;
                    DragDropEffects finalEffects = DragDrop.DoDragDrop(row, new DataObject("data", row.Item), this.AllowedEffects);
                }
            }
        }

        /// <summary>
        /// The data grid_ check drop target.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DataGrid_CheckDropTarget(object sender, DragEventArgs e)
        {
            DataGridRow row = UIHelper.FindVisualParent<DataGridRow>(this.DropTarget ?? e.OriginalSource as UIElement);
            if (row == null)
            {
                // Not over a DataGridRow
                e.Effects = DragDropEffects.None;
            }
            else
            {
                int curIndex = row.GetIndex();
                this._direction = curIndex > this._lastIndex ? DataGridDragDropDirection.Down : (curIndex < this._lastIndex ? DataGridDragDropDirection.Up : this._direction);
                this._lastIndex = curIndex;

                e.Effects = ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy) && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey ? DragDropEffects.Copy : DragDropEffects.Move;
            }

            e.Handled = true;
        }

        /// <summary>
        /// The data grid_ drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DataGrid_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            // Verify that this is a valid drop and then store the drop target
            DataGridRow row = UIHelper.FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);
            if (row != null)
            {
                this._destination = UIHelper.FindVisualParent<DataGrid>(row).ItemsSource;
                this._dropTarget = row.Item;
                if (this._dropTarget != null)
                {
                    e.Effects = ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy) && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey ? DragDropEffects.Copy : DragDropEffects.Move;
                }

                var item = e.Data.GetData("data");
                DataGridDragDropEventArgs args = new DataGridDragDropEventArgs {
                    Destination = this._destination, 
                    Direction = this._direction, 
                    DroppedObject = item, 
                    Effects = e.Effects, 
                    Source = this._source, 
                    TargetObject = this._dropTarget
                };

                if (this._dropTarget != null && this.Command != null && this.Command.CanExecute(args))
                {
                    this.Command.Execute(args);

                    this._dropTarget = null;
                    this._source = null;
                    this._destination = null;
                    this._direction = DataGridDragDropDirection.Indeterminate;
                    this._lastIndex = -1;
                }
            }
        }

    }
}

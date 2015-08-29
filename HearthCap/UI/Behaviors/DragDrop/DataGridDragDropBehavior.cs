using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace HearthCap.UI.Behaviors.DragDrop
{
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///     Creates a new command with conditional execution.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion
    }

    public class DataGridDragDropBehavior : Behavior<DataGrid>
    {
        #region Dependency Properties

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(RelayCommand<DataGridDragDropEventArgs>), typeof(DataGridDragDropBehavior), new UIPropertyMetadata(null));

        public static readonly DependencyProperty AllowedEffectsProperty =
            DependencyProperty.Register("AllowedEffects", typeof(DragDropEffects), typeof(DataGridDragDropBehavior), new UIPropertyMetadata(DragDropEffects.Move));

        public static readonly DependencyProperty DropTargetProperty =
            DependencyProperty.Register("DropTarget", typeof(DataGrid), typeof(DataGridDragDropBehavior), new UIPropertyMetadata());

        #endregion

        #region Properties

        public DragDropEffects AllowedEffects
        {
            get { return (DragDropEffects)GetValue(AllowedEffectsProperty); }
            set { SetValue(AllowedEffectsProperty, value); }
        }

        public RelayCommand<DataGridDragDropEventArgs> Command
        {
            get { return (RelayCommand<DataGridDragDropEventArgs>)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public DataGrid DropTarget
        {
            get { return (DataGrid)GetValue(DropTargetProperty); }
            set { SetValue(DropTargetProperty, value); }
        }

        #endregion

        private object _dropTarget;

        private DataGridDragDropDirection _direction = DataGridDragDropDirection.Indeterminate;
        private object _source;
        private object _destination;

        private int _lastIndex = -1;

        protected override void OnAttached()
        {
            // Mouse Move
            var mouseListener = new WeakEventListener<DataGridDragDropBehavior, DataGrid, MouseEventArgs>(this, AssociatedObject);
            mouseListener.OnEventAction = (instance, source, args) => instance.DataGrid_MouseMove(source, args);
            mouseListener.OnDetachAction = (listenerRef, source) => source.MouseMove -= listenerRef.OnEvent;
            AssociatedObject.MouseMove += mouseListener.OnEvent;

            // Drag Enter/Leave/Over
            var dragListener = new WeakEventListener<DataGridDragDropBehavior, DataGrid, DragEventArgs>(this, AssociatedObject);
            dragListener.OnEventAction = (instance, source, args) => instance.DataGrid_CheckDropTarget(source, args);
            dragListener.OnDetachAction = (listenerRef, source) =>
                {
                    source.DragEnter -= listenerRef.OnEvent;
                    source.DragLeave -= listenerRef.OnEvent;
                    source.DragOver -= listenerRef.OnEvent;
                };
            AssociatedObject.DragEnter += dragListener.OnEvent;
            AssociatedObject.DragLeave += dragListener.OnEvent;
            AssociatedObject.DragOver += dragListener.OnEvent;

            // Drop
            var target = DropTarget ?? AssociatedObject;
            var dropListener = new WeakEventListener<DataGridDragDropBehavior, DataGrid, DragEventArgs>(this, target);
            dropListener.OnEventAction = (instance, source, args) => instance.DataGrid_Drop(source, args);
            dropListener.OnDetachAction = (listenerRef, source) => source.Drop -= listenerRef.OnEvent;
            target.Drop += dropListener.OnEvent;

            base.OnAttached();
        }

        private void DataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var row = UIHelper.FindVisualParent<DataGridRow>(e.OriginalSource as FrameworkElement);
                if (row != null
                    && row.IsSelected)
                {
                    _source = UIHelper.FindVisualParent<DataGrid>(row).ItemsSource;
                    var finalEffects = System.Windows.DragDrop.DoDragDrop(row, new DataObject("data", row.Item), AllowedEffects);
                }
            }
        }

        private void DataGrid_CheckDropTarget(object sender, DragEventArgs e)
        {
            var row = UIHelper.FindVisualParent<DataGridRow>(DropTarget ?? e.OriginalSource as UIElement);
            if (row == null)
            {
                // Not over a DataGridRow
                e.Effects = DragDropEffects.None;
            }
            else
            {
                var curIndex = row.GetIndex();
                _direction = curIndex > _lastIndex ? DataGridDragDropDirection.Down : (curIndex < _lastIndex ? DataGridDragDropDirection.Up : _direction);
                _lastIndex = curIndex;

                e.Effects = ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy) && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey ? DragDropEffects.Copy : DragDropEffects.Move;
            }

            e.Handled = true;
        }

        private void DataGrid_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            // Verify that this is a valid drop and then store the drop target
            var row = UIHelper.FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);
            if (row != null)
            {
                _destination = UIHelper.FindVisualParent<DataGrid>(row).ItemsSource;
                _dropTarget = row.Item;
                if (_dropTarget != null)
                {
                    e.Effects = ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy) && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey ? DragDropEffects.Copy : DragDropEffects.Move;
                }
                var item = e.Data.GetData("data");
                var args = new DataGridDragDropEventArgs
                    {
                        Destination = _destination,
                        Direction = _direction,
                        DroppedObject = item,
                        Effects = e.Effects,
                        Source = _source,
                        TargetObject = _dropTarget
                    };

                if (_dropTarget != null
                    && Command != null
                    && Command.CanExecute(args))
                {
                    Command.Execute(args);

                    _dropTarget = null;
                    _source = null;
                    _destination = null;
                    _direction = DataGridDragDropDirection.Indeterminate;
                    _lastIndex = -1;
                }
            }
        }
    }
}

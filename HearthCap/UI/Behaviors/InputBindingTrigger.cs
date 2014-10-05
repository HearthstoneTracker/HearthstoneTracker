// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputBindingTrigger.cs" company="">
//   
// </copyright>
// <summary>
//   The input binding trigger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// The input binding trigger.
    /// </summary>
    public class InputBindingTrigger : TriggerBase<FrameworkElement>, ICommand
    {
        /// <summary>
        /// Gets or sets the input binding.
        /// </summary>
        public InputBinding InputBinding
        {
            get { return (InputBinding)this.GetValue(InputBindingProperty); }
            set { this.SetValue(InputBindingProperty, value); }
        }

        /// <summary>
        /// The input binding property.
        /// </summary>
        public static readonly DependencyProperty InputBindingProperty =
            DependencyProperty.Register("InputBinding", typeof(InputBinding)
            , typeof(InputBindingTrigger)
            , new UIPropertyMetadata(null));

        /// <summary>
        /// The on attached.
        /// </summary>
        protected override void OnAttached()
        {
            if (this.InputBinding != null)
            {
                this.InputBinding.Command = this;
                this.AssociatedObject.InputBindings.Add(this.InputBinding);
            }

            base.OnAttached();
        }

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
            // action is anyway blocked by Caliburn at the invoke level
            return true;
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this.InvokeActions(parameter);
        }

        #endregion
    }
}
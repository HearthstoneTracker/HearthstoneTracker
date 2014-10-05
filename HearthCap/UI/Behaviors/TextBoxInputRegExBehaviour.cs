// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextBoxInputRegExBehaviour.cs" company="">
//   
// </copyright>
// <summary>
//   Regular expression for Textbox with properties:
//   ,
//   ,
//   .
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    ///     Regular expression for Textbox with properties: 
    ///         <see cref="RegularExpression"/>, 
    ///         <see cref="MaxLength"/>,
    ///         <see cref="EmptyValue"/>.
    /// </summary>
    public class TextBoxInputRegExBehaviour : Behavior<TextBox>
    {
        #region DependencyProperties

        /// <summary>
        /// The regular expression property.
        /// </summary>
        public static readonly DependencyProperty RegularExpressionProperty =
                DependencyProperty.Register("TextBoxInputRegExBehaviour", typeof(string), typeof(TextBoxInputRegExBehaviour), null);

        /// <summary>
        /// Gets or sets the regular expression.
        /// </summary>
        public string RegularExpression
        {
            get { return (string)this.GetValue(RegularExpressionProperty); }
            set { this.SetValue(RegularExpressionProperty, value); }
        }

        /// <summary>
        /// The max length property.
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBoxInputRegExBehaviour), 
                                            new FrameworkPropertyMetadata(int.MinValue));

        /// <summary>
        /// Gets or sets the max length.
        /// </summary>
        public int MaxLength
        {
            get { return (int)this.GetValue(MaxLengthProperty); }
            set { this.SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// The empty value property.
        /// </summary>
        public static readonly DependencyProperty EmptyValueProperty =
            DependencyProperty.Register("EmptyValue", typeof(string), typeof(TextBoxInputRegExBehaviour), null);

        /// <summary>
        /// Gets or sets the empty value.
        /// </summary>
        public string EmptyValue
        {
            get { return (string)this.GetValue(EmptyValueProperty); }
            set { this.SetValue(EmptyValueProperty, value); }
        }
        #endregion

        /// <summary>
        ///     Attach our behaviour. Add event handlers
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.PreviewTextInput += this.PreviewTextInputHandler;
            this.AssociatedObject.PreviewKeyDown += this.PreviewKeyDownHandler;
            DataObject.AddPastingHandler(this.AssociatedObject, this.PastingHandler);
        }

        /// <summary>
        ///     Deattach our behaviour. remove event handlers
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.PreviewTextInput -= this.PreviewTextInputHandler;
            this.AssociatedObject.PreviewKeyDown -= this.PreviewKeyDownHandler;
            DataObject.RemovePastingHandler(this.AssociatedObject, this.PastingHandler);
        }

        #region Event handlers [PRIVATE] --------------------------------------

        /// <summary>
        /// The preview text input handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
        {
            string text;
            if (this.TreatSelectedText(out text))
            {
                text = text.Insert(this.AssociatedObject.CaretIndex, e.Text);
            }
            else
            {
                text = this.AssociatedObject.Text.Insert(this.AssociatedObject.CaretIndex, e.Text);
            }

            e.Handled = !this.ValidateText(text);
        }

        /// <summary>
        /// PreviewKeyDown event handler
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(this.EmptyValue))
                return;

            string text = null;

            // Handle the Backspace key
            if (e.Key == Key.Back)
            {
                if (!this.TreatSelectedText(out text))
                {
                    if (this.AssociatedObject.SelectionStart > 0)
                    {
                        text = this.AssociatedObject.Text.Remove(this.AssociatedObject.SelectionStart - 1, 1);
                    }
                }
            }
            
                // Handle the Delete key
            else if (e.Key == Key.Delete)
            {
                // If text was selected, delete it
                if (!this.TreatSelectedText(out text))
                {
                    // Otherwise delete next symbol
                    text = this.AssociatedObject.Text.Remove(this.AssociatedObject.SelectionStart, 1);
                }
            }

            if (text == string.Empty)
            {
                this.AssociatedObject.Text = this.EmptyValue;
                if (e.Key == Key.Back)
                    this.AssociatedObject.SelectionStart++;
                e.Handled = true;
            }
        }

        /// <summary>
        /// The pasting handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = Convert.ToString(e.DataObject.GetData(DataFormats.Text));

                if (!this.ValidateText(text))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }

        #endregion Event handlers [PRIVATE] -----------------------------------

        #region Auxiliary methods [PRIVATE] -----------------------------------

        /// <summary>
        /// Validate certain text by our regular expression and text length conditions
        /// </summary>
        /// <param name="text">
        /// Text for validation 
        /// </param>
        /// <returns>
        /// True - valid, False - invalid 
        /// </returns>
        private bool ValidateText(string text)
        {
            return (new Regex(this.RegularExpression, RegexOptions.IgnoreCase)).IsMatch(text) && (this.MaxLength == 0 || text.Length <= this.MaxLength);
        }

        /// <summary>
        /// Handle text selection
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// true if the character was successfully removed; otherwise, false. 
        /// </returns>
        private bool TreatSelectedText(out string text)
        {
            text = null;
            if (this.AssociatedObject.SelectionLength > 0)
            {
                text = this.AssociatedObject.Text.Remove(this.AssociatedObject.SelectionStart, this.AssociatedObject.SelectionLength);
                return true;
            }

            return false;
        }

        #endregion Auxiliary methods [PRIVATE] --------------------------------
    }
}
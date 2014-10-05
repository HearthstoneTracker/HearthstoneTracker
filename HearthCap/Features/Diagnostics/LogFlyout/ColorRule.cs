// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorRule.cs" company="">
//   
// </copyright>
// <summary>
//   The color rule.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System.Windows;
    using System.Windows.Media;

    using NLog.Conditions;

    /// <summary>
    /// The color rule.
    /// </summary>
    public class ColorRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRule"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public ColorRule(string condition = null)
            : this(condition, Brushes.Black, Brushes.Transparent, FontStyles.Normal, FontWeights.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRule"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="foregroundBrush">
        /// The foreground brush.
        /// </param>
        /// <param name="fontStyle">
        /// The font style.
        /// </param>
        public ColorRule(string condition, Brush foregroundBrush, FontStyle fontStyle)
            : this(condition, foregroundBrush, Brushes.Transparent, fontStyle, FontWeights.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRule"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="foregroundBrush">
        /// The foreground brush.
        /// </param>
        /// <param name="fontWeight">
        /// The font weight.
        /// </param>
        public ColorRule(string condition, Brush foregroundBrush, FontWeight fontWeight)
            : this(condition, foregroundBrush, Brushes.Transparent, FontStyles.Normal, fontWeight)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRule"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="foregroundBrush">
        /// The foreground brush.
        /// </param>
        /// <param name="backgroundBrush">
        /// The background brush.
        /// </param>
        public ColorRule(string condition, Brush foregroundBrush, Brush backgroundBrush)
            : this(condition, foregroundBrush, backgroundBrush, FontStyles.Normal, FontWeights.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRule"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="foregroundColor">
        /// The foreground color.
        /// </param>
        /// <param name="backgroundColor">
        /// The background color.
        /// </param>
        /// <param name="fontStyle">
        /// The font style.
        /// </param>
        /// <param name="fontWeight">
        /// The font weight.
        /// </param>
        public ColorRule(string condition, Brush foregroundColor, Brush backgroundColor, FontStyle fontStyle, FontWeight fontWeight)
        {
            this.Condition = condition;
            this.ForegroundColor = foregroundColor;
            this.BackgroundColor = backgroundColor;
            this.FontStyle = fontStyle;
            this.FontWeight = fontWeight;
        }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        public ConditionExpression Condition { get; set; }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Brush BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public Brush ForegroundColor { get; set; }
    }
}
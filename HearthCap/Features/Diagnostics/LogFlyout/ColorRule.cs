using System.Windows;
using System.Windows.Media;
using NLog.Conditions;

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    public class ColorRule
    {
        public ColorRule(string condition = null)
            : this(condition, Brushes.Black, Brushes.Transparent, FontStyles.Normal, FontWeights.Normal)
        {
        }

        public ColorRule(string condition, Brush foregroundBrush, FontStyle fontStyle)
            : this(condition, foregroundBrush, Brushes.Transparent, fontStyle, FontWeights.Normal)
        {
        }

        public ColorRule(string condition, Brush foregroundBrush, FontWeight fontWeight)
            : this(condition, foregroundBrush, Brushes.Transparent, FontStyles.Normal, fontWeight)
        {
        }

        public ColorRule(string condition, Brush foregroundBrush, Brush backgroundBrush)
            : this(condition, foregroundBrush, backgroundBrush, FontStyles.Normal, FontWeights.Normal)
        {
        }

        public ColorRule(string condition, Brush foregroundColor, Brush backgroundColor, FontStyle fontStyle, FontWeight fontWeight)
        {
            Condition = condition;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
            FontStyle = fontStyle;
            FontWeight = fontWeight;
        }

        public ConditionExpression Condition { get; set; }

        public FontStyle FontStyle { get; set; }

        public FontWeight FontWeight { get; set; }

        public Brush BackgroundColor { get; set; }

        public Brush ForegroundColor { get; set; }
    }
}

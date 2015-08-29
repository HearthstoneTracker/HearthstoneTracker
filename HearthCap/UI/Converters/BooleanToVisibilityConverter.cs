using System.Windows;

namespace HearthCap.UI.Converters
{
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter()
            :
                base(Visibility.Visible, Visibility.Collapsed)
        {
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace HearthCap.UI.Behaviors
{
    public class ExpandRowAction : TriggerAction<ToggleButton>
    {
        protected override void Invoke(object o)
        {
            var row = AssociatedObject.FindAncestor<DataGridRow>();
            if (row != null)
            {
                if (AssociatedObject.IsChecked == true)
                {
                    row.DetailsVisibility = Visibility.Visible;
                }
                else
                {
                    row.DetailsVisibility = Visibility.Collapsed;
                }
            }
        }
    }
}

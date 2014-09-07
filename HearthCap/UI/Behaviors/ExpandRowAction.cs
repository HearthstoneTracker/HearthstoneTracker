namespace HearthCap.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;

    public class ExpandRowAction : TriggerAction<ToggleButton>
    {
        protected override void Invoke(object o)
        {
            var row = this.AssociatedObject.FindAncestor<DataGridRow>();
            if (row != null)
            {
                if (this.AssociatedObject.IsChecked == true)
                    row.DetailsVisibility = Visibility.Visible;
                else
                    row.DetailsVisibility = Visibility.Collapsed;
            }
        }
    }
}
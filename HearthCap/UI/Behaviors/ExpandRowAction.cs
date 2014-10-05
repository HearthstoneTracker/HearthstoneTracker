// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandRowAction.cs" company="">
//   
// </copyright>
// <summary>
//   The expand row action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;

    /// <summary>
    /// The expand row action.
    /// </summary>
    public class ExpandRowAction : TriggerAction<ToggleButton>
    {
        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
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
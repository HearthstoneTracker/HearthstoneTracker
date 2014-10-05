// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The ui helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors.DragDrop
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// The ui helper.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// The find visual parent.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }

            return null;
        }
    }
}

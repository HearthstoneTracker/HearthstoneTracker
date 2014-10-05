// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyObjectExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The dependency object extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// The dependency object extensions.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Walk up the VisualTree, returning first parent object of the type supplied as type parameter
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                T o = obj as T;
                if (o != null)
                    return o;

                obj = VisualTreeHelper.GetParent(obj);
            }

            return null;
        }        
    }
}
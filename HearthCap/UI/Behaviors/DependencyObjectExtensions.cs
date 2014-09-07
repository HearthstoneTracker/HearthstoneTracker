namespace HearthCap.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Media;

    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Walk up the VisualTree, returning first parent object of the type supplied as type parameter
        /// </summary>
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
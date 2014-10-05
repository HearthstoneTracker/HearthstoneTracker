// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListViewBehavior.cs" company="">
//   
// </copyright>
// <summary>
//   The list view behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>The list view behavior.</summary>
    public class ListViewBehavior
    {
        /// <summary>
        /// The associations.
        /// </summary>
        static Dictionary<ListView, Capture> Associations =
            new Dictionary<ListView, Capture>();

        /// <summary>
        /// The get scroll on new item.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetScrollOnNewItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollOnNewItemProperty);
        }

        /// <summary>
        /// The set scroll on new item.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetScrollOnNewItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollOnNewItemProperty, value);
        }

        /// <summary>
        /// The scroll on new item property.
        /// </summary>
        public static readonly DependencyProperty ScrollOnNewItemProperty =
            DependencyProperty.RegisterAttached(
                "ScrollOnNewItem", 
                typeof(bool), 
                typeof(ListViewBehavior), 
                new UIPropertyMetadata(false, OnScrollOnNewItemChanged));

        /// <summary>
        /// The on scroll on new item changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void OnScrollOnNewItemChanged(
            DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            var ListView = d as ListView;
            if (ListView == null) return;
            bool oldValue = (bool)e.OldValue, newValue = (bool)e.NewValue;
            if (newValue == oldValue) return;
            if (newValue)
            {
                ListView.Loaded += ListView_Loaded;

                // ListView.Unloaded += new RoutedEventHandler(ListView_Unloaded);
            }
            else
            {
                ListView.Loaded -= ListView_Loaded;
                ListView.Unloaded -= ListView_Unloaded;
                if (Associations.ContainsKey(ListView))
                    Associations[ListView].Dispose();
            }
        }

        /// <summary>
        /// The list view_ unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        static void ListView_Unloaded(object sender, RoutedEventArgs e)
        {
            var ListView = (ListView)sender;
            if (Associations.ContainsKey(ListView))
                Associations[ListView].Dispose();
            ListView.Unloaded -= ListView_Unloaded;
        }

        /// <summary>
        /// The list view_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        static void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var ListView = (ListView)sender;
            var incc = ListView.Items as INotifyCollectionChanged;
            if (incc == null) return;
            ListView.Loaded -= ListView_Loaded;
            Associations[ListView] = new Capture(ListView);
        }

        /// <summary>
        /// The capture.
        /// </summary>
        class Capture : IDisposable
        {
            /// <summary>
            /// Gets or sets the list view.
            /// </summary>
            public ListView ListView { get; set; }

            /// <summary>
            /// Gets or sets the incc.
            /// </summary>
            public INotifyCollectionChanged incc { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Capture"/> class.
            /// </summary>
            /// <param name="ListView">
            /// The list view.
            /// </param>
            public Capture(ListView ListView)
            {
                this.ListView = ListView;
                this.incc = ListView.ItemsSource as INotifyCollectionChanged;
                if (this.incc != null)
                {
                    this.incc.CollectionChanged += this.incc_CollectionChanged;
                }
            }

            /// <summary>
            /// The incc_ collection changed.
            /// </summary>
            /// <param name="sender">
            /// The sender.
            /// </param>
            /// <param name="e">
            /// The e.
            /// </param>
            void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0)
                {
                    this.ListView.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);

                    // this.ListView.SelectedItem = e.NewItems[0];
                }
            }

            /// <summary>
            /// The dispose.
            /// </summary>
            public void Dispose()
            {
                if (this.incc != null)
                    this.incc.CollectionChanged -= this.incc_CollectionChanged;
            }
        }
    }
}
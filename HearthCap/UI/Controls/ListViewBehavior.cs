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
        static Dictionary<ListView, Capture> Associations =
            new Dictionary<ListView, Capture>();

        public static bool GetScrollOnNewItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollOnNewItemProperty);
        }

        public static void SetScrollOnNewItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollOnNewItemProperty, value);
        }

        public static readonly DependencyProperty ScrollOnNewItemProperty =
            DependencyProperty.RegisterAttached(
                "ScrollOnNewItem",
                typeof(bool),
                typeof(ListViewBehavior),
                new UIPropertyMetadata(false, OnScrollOnNewItemChanged));

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
                ListView.Loaded += new RoutedEventHandler(ListView_Loaded);
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

        static void ListView_Unloaded(object sender, RoutedEventArgs e)
        {
            var ListView = (ListView)sender;
            if (Associations.ContainsKey(ListView))
                Associations[ListView].Dispose();
            ListView.Unloaded -= ListView_Unloaded;
        }

        static void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var ListView = (ListView)sender;
            var incc = ListView.Items as INotifyCollectionChanged;
            if (incc == null) return;
            ListView.Loaded -= ListView_Loaded;
            Associations[ListView] = new Capture(ListView);
        }

        class Capture : IDisposable
        {
            public ListView ListView { get; set; }
            public INotifyCollectionChanged incc { get; set; }

            public Capture(ListView ListView)
            {
                this.ListView = ListView;
                this.incc = ListView.ItemsSource as INotifyCollectionChanged;
                if (this.incc != null)
                {
                    this.incc.CollectionChanged += this.incc_CollectionChanged;
                }
            }

            void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0)
                {
                    this.ListView.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
                    // this.ListView.SelectedItem = e.NewItems[0];
                }
            }

            public void Dispose()
            {
                if (this.incc != null)
                    this.incc.CollectionChanged -= this.incc_CollectionChanged;
            }
        }
    }
}
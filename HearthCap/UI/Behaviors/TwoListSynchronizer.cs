// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwoListSynchronizer.cs" company="">
//   
// </copyright>
// <summary>
//   A sync behaviour for a multiselector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// A sync behaviour for a multiselector.
    /// </summary>
    public static class MultiSelectorBehaviours
    {
        /// <summary>
        /// The synchronized selected items.
        /// </summary>
        public static readonly DependencyProperty SynchronizedSelectedItems = DependencyProperty.RegisterAttached(
            "SynchronizedSelectedItems", typeof(IList), typeof(MultiSelectorBehaviours), new PropertyMetadata(null, OnSynchronizedSelectedItemsChanged));

        /// <summary>
        /// The synchronization manager property.
        /// </summary>
        private static readonly DependencyProperty SynchronizationManagerProperty = DependencyProperty.RegisterAttached(
            "SynchronizationManager", typeof(SynchronizationManager), typeof(MultiSelectorBehaviours), new PropertyMetadata(null));

        /// <summary>
        /// Gets the synchronized selected items.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The list that is acting as the sync list.
        /// </returns>
        public static IList GetSynchronizedSelectedItems(DependencyObject dependencyObject)
        {
            return (IList)dependencyObject.GetValue(SynchronizedSelectedItems);
        }

        /// <summary>
        /// Sets the synchronized selected items.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value to be set as synchronized items.
        /// </param>
        public static void SetSynchronizedSelectedItems(DependencyObject dependencyObject, IList value)
        {
            dependencyObject.SetValue(SynchronizedSelectedItems, value);
        }

        /// <summary>
        /// The get synchronization manager.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="SynchronizationManager"/>.
        /// </returns>
        private static SynchronizationManager GetSynchronizationManager(DependencyObject dependencyObject)
        {
            return (SynchronizationManager)dependencyObject.GetValue(SynchronizationManagerProperty);
        }

        /// <summary>
        /// The set synchronization manager.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void SetSynchronizationManager(DependencyObject dependencyObject, SynchronizationManager value)
        {
            dependencyObject.SetValue(SynchronizationManagerProperty, value);
        }

        /// <summary>
        /// The on synchronized selected items changed.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnSynchronizedSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                SynchronizationManager synchronizer = GetSynchronizationManager(dependencyObject);
                synchronizer.StopSynchronizing();

                SetSynchronizationManager(dependencyObject, null);
            }

            IList list = e.NewValue as IList;
            Selector selector = dependencyObject as Selector;

            // check that this property is an IList, and that it is being set on a ListBox
            if (list != null && selector != null)
            {
                SynchronizationManager synchronizer = GetSynchronizationManager(dependencyObject);
                if (synchronizer == null)
                {
                    synchronizer = new SynchronizationManager(selector);
                    SetSynchronizationManager(dependencyObject, synchronizer);
                }

                synchronizer.StartSynchronizingList();
            }
        }

        /// <summary>
        /// A synchronization manager.
        /// </summary>
        private class SynchronizationManager
        {
            /// <summary>
            /// The _multi selector.
            /// </summary>
            private readonly Selector _multiSelector;

            /// <summary>
            /// The _synchronizer.
            /// </summary>
            private TwoListSynchronizer _synchronizer;

            /// <summary>
            /// Initializes a new instance of the <see cref="SynchronizationManager"/> class.
            /// </summary>
            /// <param name="selector">
            /// The selector.
            /// </param>
            internal SynchronizationManager(Selector selector)
            {
                this._multiSelector = selector;
            }

            /// <summary>
            /// Starts synchronizing the list.
            /// </summary>
            public void StartSynchronizingList()
            {
                IList list = GetSynchronizedSelectedItems(this._multiSelector);

                if (list != null)
                {
                    this._synchronizer = new TwoListSynchronizer(GetSelectedItemsCollection(this._multiSelector), list);
                    this._synchronizer.StartSynchronizing();
                }
            }

            /// <summary>
            /// Stops synchronizing the list.
            /// </summary>
            public void StopSynchronizing()
            {
                this._synchronizer.StopSynchronizing();
            }

            /// <summary>
            /// The get selected items collection.
            /// </summary>
            /// <param name="selector">
            /// The selector.
            /// </param>
            /// <returns>
            /// The <see cref="IList"/>.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// </exception>
            public static IList GetSelectedItemsCollection(Selector selector)
            {
                if (selector is MultiSelector)
                {
                    return (selector as MultiSelector).SelectedItems;
                }
                else if (selector is ListBox)
                {
                    return (selector as ListBox).SelectedItems;
                }
                else
                {
                    throw new InvalidOperationException("Target object has no SelectedItems property to bind.");
                }
            }

        }
    }

    /// <summary>
    /// Converts items in the Master list to Items in the target list, and back again.
    /// </summary>
    public interface IListItemConverter
    {
        /// <summary>
        /// Converts the specified master list item.
        /// </summary>
        /// <param name="masterListItem">
        /// The master list item.
        /// </param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        object Convert(object masterListItem);

        /// <summary>
        /// Converts the specified target list item.
        /// </summary>
        /// <param name="targetListItem">
        /// The target list item.
        /// </param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        object ConvertBack(object targetListItem);
    }

    /// <summary>
    /// Keeps two lists synchronized. 
    /// </summary>
    public class TwoListSynchronizer : IWeakEventListener
    {
        /// <summary>
        /// The default converter.
        /// </summary>
        private static readonly IListItemConverter DefaultConverter = new DoNothingListItemConverter();

        /// <summary>
        /// The _master list.
        /// </summary>
        private readonly IList _masterList;

        /// <summary>
        /// The _master target converter.
        /// </summary>
        private readonly IListItemConverter _masterTargetConverter;

        /// <summary>
        /// The _target list.
        /// </summary>
        private readonly IList _targetList;


        /// <summary>
        /// Initializes a new instance of the <see cref="TwoListSynchronizer"/> class.
        /// </summary>
        /// <param name="masterList">
        /// The master list.
        /// </param>
        /// <param name="targetList">
        /// The target list.
        /// </param>
        /// <param name="masterTargetConverter">
        /// The master-target converter.
        /// </param>
        public TwoListSynchronizer(IList masterList, IList targetList, IListItemConverter masterTargetConverter)
        {
            this._masterList = masterList;
            this._targetList = targetList;
            this._masterTargetConverter = masterTargetConverter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoListSynchronizer"/> class.
        /// </summary>
        /// <param name="masterList">
        /// The master list.
        /// </param>
        /// <param name="targetList">
        /// The target list.
        /// </param>
        public TwoListSynchronizer(IList masterList, IList targetList)
            : this(masterList, targetList, DefaultConverter)
        {
        }

        /// <summary>
        /// The change list action.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="converter">
        /// The converter.
        /// </param>
        private delegate void ChangeListAction(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter);

        /// <summary>
        /// Starts synchronizing the lists.
        /// </summary>
        public void StartSynchronizing()
        {
            this.ListenForChangeEvents(this._masterList);
            this.ListenForChangeEvents(this._targetList);

            // Update the Target list from the Master list
            this.SetListValuesFromSource(this._masterList, this._targetList, this.ConvertFromMasterToTarget);

            // In some cases the target list might have its own view on which items should included:
            // so update the master list from the target list
            // (This is the case with a ListBox SelectedItems collection: only items from the ItemsSource can be included in SelectedItems)
            if (!this.TargetAndMasterCollectionsAreEqual())
            {
                this.SetListValuesFromSource(this._targetList, this._masterList, this.ConvertFromTargetToMaster);
            }
        }

        /// <summary>
        /// Stop synchronizing the lists.
        /// </summary>
        public void StopSynchronizing()
        {
            this.StopListeningForChangeEvents(this._masterList);
            this.StopListeningForChangeEvents(this._targetList);
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <param name="managerType">
        /// The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.
        /// </param>
        /// <param name="sender">
        /// Object that originated the event.
        /// </param>
        /// <param name="e">
        /// Event data.
        /// </param>
        /// <returns>
        /// true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            this.HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);

            return true;
        }

        /// <summary>
        /// Listens for change events on a list.
        /// </summary>
        /// <param name="list">
        /// The list to listen to.
        /// </param>
        protected void ListenForChangeEvents(IList list)
        {
            if (list is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, this);
            }
        }

        /// <summary>
        /// Stops listening for change events.
        /// </summary>
        /// <param name="list">
        /// The list to stop listening to.
        /// </param>
        protected void StopListeningForChangeEvents(IList list)
        {
            if (list is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.RemoveListener(list as INotifyCollectionChanged, this);
            }
        }

        /// <summary>
        /// The add items.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="converter">
        /// The converter.
        /// </param>
        private void AddItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int itemCount = e.NewItems.Count;

            for (int i = 0; i < itemCount; i++)
            {
                int insertionPoint = e.NewStartingIndex + i;

                if (insertionPoint > list.Count)
                {
                    list.Add(converter(e.NewItems[i]));
                }
                else
                {
                    list.Insert(insertionPoint, converter(e.NewItems[i]));
                }
            }
        }

        /// <summary>
        /// The convert from master to target.
        /// </summary>
        /// <param name="masterListItem">
        /// The master list item.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object ConvertFromMasterToTarget(object masterListItem)
        {
            return this._masterTargetConverter == null ? masterListItem : this._masterTargetConverter.Convert(masterListItem);
        }

        /// <summary>
        /// The convert from target to master.
        /// </summary>
        /// <param name="targetListItem">
        /// The target list item.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object ConvertFromTargetToMaster(object targetListItem)
        {
            return this._masterTargetConverter == null ? targetListItem : this._masterTargetConverter.ConvertBack(targetListItem);
        }

        /// <summary>
        /// The handle collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList sourceList = sender as IList;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.PerformActionOnAllLists(this.AddItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.PerformActionOnAllLists(this.MoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.PerformActionOnAllLists(this.RemoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.PerformActionOnAllLists(this.ReplaceItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.UpdateListsFromSource(sender as IList);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// The move items.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="converter">
        /// The converter.
        /// </param>
        private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            this.RemoveItems(list, e, converter);
            this.AddItems(list, e, converter);
        }

        /// <summary>
        /// The perform action on all lists.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="sourceList">
        /// The source list.
        /// </param>
        /// <param name="collectionChangedArgs">
        /// The collection changed args.
        /// </param>
        private void PerformActionOnAllLists(ChangeListAction action, IList sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
        {
            if (sourceList == this._masterList)
            {
                this.PerformActionOnList(this._targetList, action, collectionChangedArgs, this.ConvertFromMasterToTarget);
            }
            else
            {
                this.PerformActionOnList(this._masterList, action, collectionChangedArgs, this.ConvertFromTargetToMaster);
            }
        }

        /// <summary>
        /// The perform action on list.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="collectionChangedArgs">
        /// The collection changed args.
        /// </param>
        /// <param name="converter">
        /// The converter.
        /// </param>
        private void PerformActionOnList(IList list, ChangeListAction action, NotifyCollectionChangedEventArgs collectionChangedArgs, Converter<object, object> converter)
        {
            this.StopListeningForChangeEvents(list);
            action(list, collectionChangedArgs, converter);
            this.ListenForChangeEvents(list);
        }

        /// <summary>
        /// The remove items.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="converter">
        /// The converter.
        /// </param>
        private void RemoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int itemCount = e.OldItems.Count;

            // for the number of items being removed, remove the item from the Old Starting Index
            // (this will cause following items to be shifted down to fill the hole).
            for (int i = 0; i < itemCount; i++)
            {
                list.RemoveAt(e.OldStartingIndex);
            }
        }

        /// <summary>
        /// The replace items.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="converter">
        /// The converter.
        /// </param>
        private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            this.RemoveItems(list, e, converter);
            this.AddItems(list, e, converter);
        }

        /// <summary>
        /// The set list values from source.
        /// </summary>
        /// <param name="sourceList">
        /// The source list.
        /// </param>
        /// <param name="targetList">
        /// The target list.
        /// </param>
        /// <param name="converter">
        /// The converter.
        /// </param>
        private void SetListValuesFromSource(IList sourceList, IList targetList, Converter<object, object> converter)
        {
            this.StopListeningForChangeEvents(targetList);

            targetList.Clear();

            foreach (object o in sourceList)
            {
                targetList.Add(converter(o));
            }

            this.ListenForChangeEvents(targetList);
        }

        /// <summary>
        /// The target and master collections are equal.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TargetAndMasterCollectionsAreEqual()
        {
            return this._masterList.Cast<object>().SequenceEqual(this._targetList.Cast<object>().Select(item => this.ConvertFromTargetToMaster(item)));
        }

        /// <summary>
        /// Makes sure that all synchronized lists have the same values as the source list.
        /// </summary>
        /// <param name="sourceList">
        /// The source list.
        /// </param>
        private void UpdateListsFromSource(IList sourceList)
        {
            if (sourceList == this._masterList)
            {
                this.SetListValuesFromSource(this._masterList, this._targetList, this.ConvertFromMasterToTarget);
            }
            else
            {
                this.SetListValuesFromSource(this._targetList, this._masterList, this.ConvertFromTargetToMaster);
            }
        }




        /// <summary>
        /// An implementation that does nothing in the conversions.
        /// </summary>
        internal class DoNothingListItemConverter : IListItemConverter
        {
            /// <summary>
            /// Converts the specified master list item.
            /// </summary>
            /// <param name="masterListItem">
            /// The master list item.
            /// </param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public object Convert(object masterListItem)
            {
                return masterListItem;
            }

            /// <summary>
            /// Converts the specified target list item.
            /// </summary>
            /// <param name="targetListItem">
            /// The target list item.
            /// </param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public object ConvertBack(object targetListItem)
            {
                return targetListItem;
            }
        }
    }
}
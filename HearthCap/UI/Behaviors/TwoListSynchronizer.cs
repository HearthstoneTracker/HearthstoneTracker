using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace HearthCap.UI.Behaviors
{
    /// <summary>
    ///     A sync behaviour for a multiselector.
    /// </summary>
    public static class MultiSelectorBehaviours
    {
        public static readonly DependencyProperty SynchronizedSelectedItems = DependencyProperty.RegisterAttached(
            "SynchronizedSelectedItems", typeof(IList), typeof(MultiSelectorBehaviours), new PropertyMetadata(null, OnSynchronizedSelectedItemsChanged));

        private static readonly DependencyProperty SynchronizationManagerProperty = DependencyProperty.RegisterAttached(
            "SynchronizationManager", typeof(SynchronizationManager), typeof(MultiSelectorBehaviours), new PropertyMetadata(null));

        /// <summary>
        ///     Gets the synchronized selected items.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The list that is acting as the sync list.</returns>
        public static IList GetSynchronizedSelectedItems(DependencyObject dependencyObject)
        {
            return (IList)dependencyObject.GetValue(SynchronizedSelectedItems);
        }

        /// <summary>
        ///     Sets the synchronized selected items.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">The value to be set as synchronized items.</param>
        public static void SetSynchronizedSelectedItems(DependencyObject dependencyObject, IList value)
        {
            dependencyObject.SetValue(SynchronizedSelectedItems, value);
        }

        private static SynchronizationManager GetSynchronizationManager(DependencyObject dependencyObject)
        {
            return (SynchronizationManager)dependencyObject.GetValue(SynchronizationManagerProperty);
        }

        private static void SetSynchronizationManager(DependencyObject dependencyObject, SynchronizationManager value)
        {
            dependencyObject.SetValue(SynchronizationManagerProperty, value);
        }

        private static void OnSynchronizedSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                var synchronizer = GetSynchronizationManager(dependencyObject);
                synchronizer.StopSynchronizing();

                SetSynchronizationManager(dependencyObject, null);
            }

            var list = e.NewValue as IList;
            var selector = dependencyObject as Selector;

            // check that this property is an IList, and that it is being set on a ListBox
            if (list != null
                && selector != null)
            {
                var synchronizer = GetSynchronizationManager(dependencyObject);
                if (synchronizer == null)
                {
                    synchronizer = new SynchronizationManager(selector);
                    SetSynchronizationManager(dependencyObject, synchronizer);
                }

                synchronizer.StartSynchronizingList();
            }
        }

        /// <summary>
        ///     A synchronization manager.
        /// </summary>
        private class SynchronizationManager
        {
            private readonly Selector _multiSelector;
            private TwoListSynchronizer _synchronizer;

            /// <summary>
            ///     Initializes a new instance of the <see cref="SynchronizationManager" /> class.
            /// </summary>
            /// <param name="selector">The selector.</param>
            internal SynchronizationManager(Selector selector)
            {
                _multiSelector = selector;
            }

            /// <summary>
            ///     Starts synchronizing the list.
            /// </summary>
            public void StartSynchronizingList()
            {
                var list = GetSynchronizedSelectedItems(_multiSelector);

                if (list != null)
                {
                    _synchronizer = new TwoListSynchronizer(GetSelectedItemsCollection(_multiSelector), list);
                    _synchronizer.StartSynchronizing();
                }
            }

            /// <summary>
            ///     Stops synchronizing the list.
            /// </summary>
            public void StopSynchronizing()
            {
                _synchronizer.StopSynchronizing();
            }

            public static IList GetSelectedItemsCollection(Selector selector)
            {
                if (selector is MultiSelector)
                {
                    return (selector as MultiSelector).SelectedItems;
                }
                if (selector is ListBox)
                {
                    return (selector as ListBox).SelectedItems;
                }
                throw new InvalidOperationException("Target object has no SelectedItems property to bind.");
            }
        }
    }

    /// <summary>
    ///     Converts items in the Master list to Items in the target list, and back again.
    /// </summary>
    public interface IListItemConverter
    {
        /// <summary>
        ///     Converts the specified master list item.
        /// </summary>
        /// <param name="masterListItem">The master list item.</param>
        /// <returns>The result of the conversion.</returns>
        object Convert(object masterListItem);

        /// <summary>
        ///     Converts the specified target list item.
        /// </summary>
        /// <param name="targetListItem">The target list item.</param>
        /// <returns>The result of the conversion.</returns>
        object ConvertBack(object targetListItem);
    }

    /// <summary>
    ///     Keeps two lists synchronized.
    /// </summary>
    public class TwoListSynchronizer : IWeakEventListener
    {
        private static readonly IListItemConverter DefaultConverter = new DoNothingListItemConverter();
        private readonly IList _masterList;
        private readonly IListItemConverter _masterTargetConverter;
        private readonly IList _targetList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TwoListSynchronizer" /> class.
        /// </summary>
        /// <param name="masterList">The master list.</param>
        /// <param name="targetList">The target list.</param>
        /// <param name="masterTargetConverter">The master-target converter.</param>
        public TwoListSynchronizer(IList masterList, IList targetList, IListItemConverter masterTargetConverter)
        {
            _masterList = masterList;
            _targetList = targetList;
            _masterTargetConverter = masterTargetConverter;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TwoListSynchronizer" /> class.
        /// </summary>
        /// <param name="masterList">The master list.</param>
        /// <param name="targetList">The target list.</param>
        public TwoListSynchronizer(IList masterList, IList targetList)
            : this(masterList, targetList, DefaultConverter)
        {
        }

        private delegate void ChangeListAction(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter);

        /// <summary>
        ///     Starts synchronizing the lists.
        /// </summary>
        public void StartSynchronizing()
        {
            ListenForChangeEvents(_masterList);
            ListenForChangeEvents(_targetList);

            // Update the Target list from the Master list
            SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);

            // In some cases the target list might have its own view on which items should included:
            // so update the master list from the target list
            // (This is the case with a ListBox SelectedItems collection: only items from the ItemsSource can be included in SelectedItems)
            if (!TargetAndMasterCollectionsAreEqual())
            {
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
            }
        }

        /// <summary>
        ///     Stop synchronizing the lists.
        /// </summary>
        public void StopSynchronizing()
        {
            StopListeningForChangeEvents(_masterList);
            StopListeningForChangeEvents(_targetList);
        }

        /// <summary>
        ///     Receives events from the centralized event manager.
        /// </summary>
        /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager" /> calling this method.</param>
        /// <param name="sender">Object that originated the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>
        ///     true if the listener handled the event. It is considered an error by the
        ///     <see cref="T:System.Windows.WeakEventManager" /> handling in WPF to register a listener for an event that the
        ///     listener does not handle. Regardless, the method should return false if it receives an event that it does not
        ///     recognize or handle.
        /// </returns>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);

            return true;
        }

        /// <summary>
        ///     Listens for change events on a list.
        /// </summary>
        /// <param name="list">The list to listen to.</param>
        protected void ListenForChangeEvents(IList list)
        {
            if (list is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, this);
            }
        }

        /// <summary>
        ///     Stops listening for change events.
        /// </summary>
        /// <param name="list">The list to stop listening to.</param>
        protected void StopListeningForChangeEvents(IList list)
        {
            if (list is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.RemoveListener(list as INotifyCollectionChanged, this);
            }
        }

        private void AddItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            var itemCount = e.NewItems.Count;

            for (var i = 0; i < itemCount; i++)
            {
                var insertionPoint = e.NewStartingIndex + i;

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

        private object ConvertFromMasterToTarget(object masterListItem)
        {
            return _masterTargetConverter == null ? masterListItem : _masterTargetConverter.Convert(masterListItem);
        }

        private object ConvertFromTargetToMaster(object targetListItem)
        {
            return _masterTargetConverter == null ? targetListItem : _masterTargetConverter.ConvertBack(targetListItem);
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var sourceList = sender as IList;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    PerformActionOnAllLists(AddItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    PerformActionOnAllLists(MoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    PerformActionOnAllLists(RemoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    PerformActionOnAllLists(ReplaceItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateListsFromSource(sender as IList);
                    break;
                default:
                    break;
            }
        }

        private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void PerformActionOnAllLists(ChangeListAction action, IList sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
        {
            if (sourceList == _masterList)
            {
                PerformActionOnList(_targetList, action, collectionChangedArgs, ConvertFromMasterToTarget);
            }
            else
            {
                PerformActionOnList(_masterList, action, collectionChangedArgs, ConvertFromTargetToMaster);
            }
        }

        private void PerformActionOnList(IList list, ChangeListAction action, NotifyCollectionChangedEventArgs collectionChangedArgs, Converter<object, object> converter)
        {
            StopListeningForChangeEvents(list);
            action(list, collectionChangedArgs, converter);
            ListenForChangeEvents(list);
        }

        private void RemoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            var itemCount = e.OldItems.Count;

            // for the number of items being removed, remove the item from the Old Starting Index
            // (this will cause following items to be shifted down to fill the hole).
            for (var i = 0; i < itemCount; i++)
            {
                list.RemoveAt(e.OldStartingIndex);
            }
        }

        private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void SetListValuesFromSource(IList sourceList, IList targetList, Converter<object, object> converter)
        {
            StopListeningForChangeEvents(targetList);

            targetList.Clear();

            foreach (var o in sourceList)
            {
                targetList.Add(converter(o));
            }

            ListenForChangeEvents(targetList);
        }

        private bool TargetAndMasterCollectionsAreEqual()
        {
            return _masterList.Cast<object>().SequenceEqual(_targetList.Cast<object>().Select(item => ConvertFromTargetToMaster(item)));
        }

        /// <summary>
        ///     Makes sure that all synchronized lists have the same values as the source list.
        /// </summary>
        /// <param name="sourceList">The source list.</param>
        private void UpdateListsFromSource(IList sourceList)
        {
            if (sourceList == _masterList)
            {
                SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);
            }
            else
            {
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
            }
        }

        /// <summary>
        ///     An implementation that does nothing in the conversions.
        /// </summary>
        internal class DoNothingListItemConverter : IListItemConverter
        {
            /// <summary>
            ///     Converts the specified master list item.
            /// </summary>
            /// <param name="masterListItem">The master list item.</param>
            /// <returns>The result of the conversion.</returns>
            public object Convert(object masterListItem)
            {
                return masterListItem;
            }

            /// <summary>
            ///     Converts the specified target list item.
            /// </summary>
            /// <param name="targetListItem">The target list item.</param>
            /// <returns>The result of the conversion.</returns>
            public object ConvertBack(object targetListItem)
            {
                return targetListItem;
            }
        }
    }
}

//namespace HearthCap.Features.TextFiles
//{
//    using System;
//    using System.Collections;
//    using System.Collections.Generic;
//    using System.ComponentModel.Composition;
//    using System.Linq;

//    using Caliburn.Micro;

//    public class TextFilesListenerCollection : ICollection<TextFilesEventsListener>
//    {
//        private readonly IEventAggregator events;

//        private readonly IObservableCollection<TextFileModel> templates;
//        private IList<TextFilesEventsListener> innerList = new List<TextFilesEventsListener>();

//        public TextFilesListenerCollection(IEventAggregator events, IObservableCollection<TextFileModel> templates)
//        {
//            if (events == null)
//            {
//                throw new ArgumentNullException("events");
//            }
//            if (templates == null)
//            {
//                throw new ArgumentNullException("templates");
//            }
//            this.events = events;
//            this.templates = templates;

//            // Default listeners:
//            Add(new ArenaEventsListener(templates));
//        }

//        /// <summary>
//        /// Returns an enumerator that iterates through the collection.
//        /// </summary>
//        /// <returns>
//        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
//        /// </returns>
//        public IEnumerator<TextFilesEventsListener> GetEnumerator()
//        {
//            return this.innerList.GetEnumerator();
//        }

//        /// <summary>
//        /// Returns an enumerator that iterates through a collection.
//        /// </summary>
//        /// <returns>
//        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
//        /// </returns>
//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return ((IEnumerable)this.innerList).GetEnumerator();
//        }

//        /// <summary>
//        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </summary>
//        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
//        public void Add(TextFilesEventsListener item)
//        {
//            this.innerList.Add(item);
//            this.events.Subscribe(item);
//        }

//        /// <summary>
//        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </summary>
//        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
//        public void Clear()
//        {
//            foreach (var listener in this.innerList)
//            {
//                this.events.Unsubscribe(listener);
//            }
//            this.innerList.Clear();
//        }

//        /// <summary>
//        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
//        /// </summary>
//        /// <returns>
//        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
//        /// </returns>
//        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
//        public bool Contains(TextFilesEventsListener item)
//        {
//            return this.innerList.Contains(item);
//        }

//        /// <summary>
//        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
//        /// </summary>
//        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
//        public void CopyTo(TextFilesEventsListener[] array, int arrayIndex)
//        {
//            this.innerList.CopyTo(array, arrayIndex);
//        }

//        /// <summary>
//        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </summary>
//        /// <returns>
//        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </returns>
//        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
//        public bool Remove(TextFilesEventsListener item)
//        {
//            this.events.Unsubscribe(item);
//            return this.innerList.Remove(item);
//        }

//        /// <summary>
//        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </summary>
//        /// <returns>
//        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </returns>
//        public int Count
//        {
//            get
//            {
//                return this.innerList.Count;
//            }
//        }

//        /// <summary>
//        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
//        /// </summary>
//        /// <returns>
//        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
//        /// </returns>
//        public bool IsReadOnly
//        {
//            get
//            {
//                return this.innerList.IsReadOnly;
//            }
//        }
//    }
//}
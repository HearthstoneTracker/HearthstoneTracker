// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The list extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games
{
    using System;
    using System.Collections.Generic;

    using Caliburn.Micro;

    /// <summary>
    /// The list extensions.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// The replace.
        /// </summary>
        /// <param name="lst">
        /// The lst.
        /// </param>
        /// <param name="expr">
        /// The expr.
        /// </param>
        /// <param name="newItem">
        /// The new item.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void Replace<T>(this BindableCollection<T> lst, Func<T, bool> expr, T newItem)
        {
            for (int i = lst.Count - 1; i >= 0; i--)
            {
                if (expr(lst[i]))
                {
                    lst.RemoveAt(i);
                    if (i <= lst.Count)
                    {
                        lst.Insert(i, newItem);
                    }
                    else
                    {
                        lst.Add(newItem);
                    }

                    // lst[i] = newItem;
                    break;
                }
            }
        }

        /// <summary>
        /// The find index.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static int FindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int i = 0;
            foreach (var val in source)
            {
                if (predicate(val)) return i;
                i++;
            }

            return -1;
        }
    }
}
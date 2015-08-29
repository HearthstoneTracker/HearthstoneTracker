using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace HearthCap.Features.Games
{
    public static class ListExtensions
    {
        public static void Replace<T>(this BindableCollection<T> lst, Func<T, bool> expr, T newItem)
        {
            for (var i = lst.Count - 1; i >= 0; i--)
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

        public static int FindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var i = 0;
            foreach (var val in source)
            {
                if (predicate(val))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
    }
}

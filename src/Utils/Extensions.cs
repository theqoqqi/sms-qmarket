using System.Collections.Generic;

namespace QMarketPlugin.Utils; 

public static class Extensions {

    public static void InsertAfter<T>(this IList<T> list, T item, T after) {
        list.Insert(list.IndexOf(after) + 1, item);
    }
}

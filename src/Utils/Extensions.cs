using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace QMarketPlugin.Utils; 

public static class Extensions {

    public static void InsertAfter<T>(this IList<T> list, T item, T after) {
        list.Insert(list.IndexOf(after) + 1, item);
    }

    public static string AsHtmlTag(this Color color) {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
    }

    public static int Random(this RangedInt range, System.Random random) {
        return random.Next(range.Min, range.Max + 1);
    }
}

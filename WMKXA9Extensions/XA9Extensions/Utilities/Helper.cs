using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XA9Extensions.Utilities
{
    /// <summary>
    /// Returns true when the number is in between start and end
    /// </summary>
    public static class Helper
    {
        public static bool IsBetween<T>(this T item, T start, T end)
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                && Comparer<T>.Default.Compare(item, end) <= 0;
        }
    }
}

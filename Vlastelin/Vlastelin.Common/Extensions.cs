using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;
using Vlastelin.Common;
using System.ComponentModel;

namespace Vlastelin.Common
{
    public static class Extensions
    {
        public static String GetTimeString(this DateTime time)
        {
            return time.ToString("HH:mm", Ct.RussianCulture);
        }

        public static DateTime SetTime(this DateTime source, DateTime time)
        {
            return new DateTime(source.Year, source.Month, source.Day, time.Hour, time.Minute, time.Second);
        }

        public static void RemoveArray<T>(this IList<T> source, IList<T> array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                source.Remove(array[i]);
            }
        }

        public static bool ArrayEquals<T>(this IEnumerable<T> source, IEnumerable<T> array)
        {
            if (source.Count() != array.Count())
            {
                return false;
            }

            for (int i = 0; i < source.Count(); i++)
            {
                if (!source.ElementAt(i).Equals(array.ElementAt(i))) return false;
            }
            return true;
        }
    }
}

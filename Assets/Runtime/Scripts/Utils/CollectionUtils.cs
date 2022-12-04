using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TMUtils.Utils.Collections
{
    public static class CollectionUtils
    {
        public static bool IsValidIndex<T>(this List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static void AddRange<T>(this List<T> list, int count, T element = default)
        {
            int newCount = list.Count + count;
            if (newCount > list.Capacity)
            {
                list.Capacity = newCount;
            }

            for (int i = 0; i < count; i++)
            {
                list.Add(element);
            }
        }

        public static void Resize<T>(this List<T> list, int size, T element = default)
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)
                    list.Capacity = size;

                list.AddRange(size - count, element);
            }
        }

        public static void GetKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> outKeys)
        {
            outKeys.Clear();
            outKeys.Capacity = dict.Keys.Count;

            foreach (var key in dict.Keys)
            {
                outKeys.Add(key);
            }
        }

        public static T[] Concat<T>(this T[] left, params T[] right)
        {
            var result = new T[left.Length + right.Length];
            left.CopyTo(result, 0);
            right.CopyTo(result, left.Length);
            return result;
        }

        public static T[] Concat<T>(this T[] a1, T[] a2, T[] a3)
        {
            int resultSize = a1.Length + a2.Length + a3.Length;
            var result = new T[resultSize];
            a1.CopyTo(result, 0);
            a2.CopyTo(result, a1.Length);
            a3.CopyTo(result, a1.Length + a2.Length);
            return result;
        }

        public static Stream ToStream(this string value) => ToStream(value, Encoding.UTF8);

        public static Stream ToStream(this string value, Encoding encoding)
            => new MemoryStream(encoding.GetBytes(value ?? string.Empty));

        public static bool Contains<T>(this T[] arr, T value) => Array.IndexOf(arr, value) != -1;
    }
}

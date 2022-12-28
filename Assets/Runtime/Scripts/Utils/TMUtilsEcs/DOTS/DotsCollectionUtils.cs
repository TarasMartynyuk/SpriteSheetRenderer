using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Debug = UnityEngine.Debug;

namespace SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs.DOTS
{
    public static class DotsCollectionUtils
    {
        public static bool IsValidIndex<T>(this NativeArray<T> array, int index) where T : unmanaged
        {
            return index >= 0 && index < array.Length;
        }

        public static bool IsValidIndex<T>(this NativeList<T> list, int index) where T : unmanaged
        {
            return index >= 0 && index < list.Length;
        }

        public static bool IsValidIndex<T>(this DynamicBuffer<T> buffer, int index) where T : unmanaged
        {
            return index >= 0 && index < buffer.Length;
        }

    
        public static bool Exists<T>(this NativeArray<T> buffer, Predicate<T> predicate)
            where T : unmanaged
        {
            return buffer.FindIndex(predicate) != -1;
        }

        public static bool Exists<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate)
            where T : unmanaged
        {
            return buffer.AsNativeArray().Exists(predicate);
        }

        public static bool All<T>(this NativeSlice<T> slice, Predicate<T> predicate)
            where T : unmanaged
        {
            foreach (var item in slice)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool All<T>(this NativeArray<T> array, Predicate<T> predicate)
            where T : unmanaged
            => new NativeSlice<T>(array).All(predicate);

        public static bool All<T>(this NativeList<T> list, Predicate<T> predicate)
            where T : unmanaged
            => new NativeSlice<T>(list).All(predicate);

        public static bool All<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate)
            where T : unmanaged
            => buffer.AsNativeArray().All(predicate);


        // non-performant, for debug 
        public static bool AllUnique<T>(this NativeArray<T> array)
            where T : unmanaged
        {
            var diffChecker = new HashSet<T>();

            foreach (var element in array)
            {
                if (!diffChecker.Add(element))
                {
                    return false;
                }
            }
            return true;
        }

        public static int FindIndex<T>(this NativeSlice<T> slice, Predicate<T> predicate, int startIndex = 0) where T : unmanaged
        {
            for (var i = startIndex; i < slice.Length; i++)
            {
                if (predicate(slice[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int FindIndex<T>(this NativeArray<T> array, Predicate<T> predicate, int startIndex = 0) where T : unmanaged
            => new NativeSlice<T>(array).FindIndex(predicate, startIndex);

        public static int FindIndex<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, int startIndex = 0) where T : unmanaged
            => buffer.AsNativeArray().FindIndex(predicate, startIndex);

        public static int FindLastIndex<T>(this NativeSlice<T> slice, Predicate<T> predicate, int startIndex = -1) where T : unmanaged
        {
            if (startIndex == -1)
            {
                startIndex = slice.Length - 1;
            }

            for (var i = startIndex; i >= 0; i--)
            {
                if (predicate(slice[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int FindLastIndex<T>(this NativeArray<T> array, Predicate<T> predicate, int startIndex = -1) where T : unmanaged
            => new NativeSlice<T>(array).FindLastIndex(predicate, startIndex);

        public static int FindLastIndex<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, int startIndex = -1) where T : unmanaged
            => buffer.AsNativeArray().FindLastIndex(predicate, startIndex);

        public static T Find<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged
        {
            var index = buffer.FindIndex(predicate);
            if (index == -1)
            {
                Debug.LogError("No element found");
                return default;
            }
            return buffer[index];
        }

        public static int IndexOf<T>(this DynamicBuffer<T> buffer, T element) where T : unmanaged, IEquatable<T>
            => buffer.AsNativeArray().IndexOf(element);

        public static bool Contains<T>(this DynamicBuffer<T> buffer, T element) where T : unmanaged, IEquatable<T> =>
            buffer.AsNativeArray().Contains(element);

        public static bool Contains<T>(this NativeList<T> list, T element) where T : unmanaged, IEquatable<T> =>
            list.AsArray().Contains(element);

        public static bool Contains<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged =>
            buffer.AsNativeArray().FindIndex(predicate) != -1;

        public static void RemoveSwapBack<T>(this DynamicBuffer<T> buffer, T value)
            where T : unmanaged, IEquatable<T> =>
            buffer.RemoveAtSwapBack(buffer.IndexOf(value));

        public static void Fill<T>(this NativeArray<T> array, T value)
            where T : unmanaged
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        public static void Fill<T>(this DynamicBuffer<T> buffer, T value)
            where T : unmanaged =>
            buffer.AsNativeArray().Fill(value);

        public static int Sum<T>(this NativeArray<T> array, Func<T, int> selector)
            where T : unmanaged
        {
            var sum = 0;
            foreach (var item in array)
            {
                sum += selector(item);
            }
            return sum;
        }

        public static int Sum<T>(this DynamicBuffer<T> buffer, Func<T, int> selector)
            where T : unmanaged =>
            buffer.AsNativeArray().Sum(selector);

        public static int Count<T>(this NativeArray<T> array, Predicate<T> predicate)
            where T : unmanaged
        {
            var count = 0;
            foreach (var element in array)
            {
                if (predicate(element))
                {
                    count++;
                }
            }
            return count;
        }

        public static int Count<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate)
            where T : unmanaged
            => buffer.AsNativeArray().Count(predicate);

        public static void AddRange<T>(this DynamicBuffer<T> buffer, int count, T element = default)
            where T : unmanaged
        {
            buffer.Capacity += count;
            for (var i = 0; i < count; i++)
            {
                buffer.Add(element);
            }
        }

        public static DynamicBuffer<TReinterpreted> GetReinterprettedBuffer<TReinterpreted, T>(Entity entity)
            where TReinterpreted : unmanaged
            where T : unmanaged, IBufferElementData
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var inputStorage = entityManager.GetBuffer<T>(entity);
            return inputStorage.Reinterpret<TReinterpreted>();
        }

        // public static unsafe ref T ElementAt<T>(this NativeArray<T> array, int index)
        //     where T : unmanaged
        // {
        //     CheckIndexInRange(index, array.Length);
        //     void* ptr = array.GetUnsafePtr();
        //     return ref UnsafeUtility.ArrayElementAsRef<T>(ptr, index);
        // }
        //
        // public static unsafe ref T ElementAtReadOnly<T>(this NativeArray<T> array, int index)
        //     where T : unmanaged
        // {
        //     CheckIndexInRange(index, array.Length);
        //     void* ptr = array.GetUnsafeReadOnlyPtr();
        //     return ref UnsafeUtility.ArrayElementAsRef<T>(ptr, index);
        // }

        public static int LastIndex<T>(this DynamicBuffer<T> buffer)
            where T : unmanaged
            => buffer.Length - 1;

        public static int LastIndex<T>(this NativeList<T> list)
            where T : unmanaged
            => list.Length - 1;

        public static int LastIndex<T>(this NativeArray<T> array)
            where T : unmanaged
            => array.Length - 1;

        public static ref T LastElement<T>(this DynamicBuffer<T> buffer)
            where T : unmanaged
            => ref buffer.ElementAt(buffer.Length - 1);

        public static ref T LastElement<T>(this NativeList<T> list)
            where T : unmanaged
            => ref list.ElementAt(list.LastIndex());

        // public static ref T LastElement<T>(this NativeArray<T> array)
        //     where T : unmanaged
        //     => ref array.ElementAt(array.LastIndex());

        public static void Resize<T>(this DynamicBuffer<T> buffer, int length, T? element = null)
            where T : unmanaged
        {
            buffer.ResizeUninitialized(length);
            var elementRaw = element.GetValueOrDefault();
            for (var i = 0; i < length; i++)
            {
                buffer[i] = elementRaw;
            }
        }

        // public static void Reverse<T>(this NativeArray<T> array)
        //     where T : unmanaged
        // {
        //     for (int i = 0; i < array.Length / 2; i++)
        //     {
        //         GenericAlgorithms.Swap(ref array.ElementAt(i), ref array.ElementAt(array.Length - i - 1));
        //     }
        // }

        public static void Reverse<T>(this NativeList<T> list)
            where T : unmanaged => list.AsArray().Reverse();

        public static void Reverse<T>(this DynamicBuffer<T> buffer)
            where T : unmanaged => buffer.AsNativeArray().Reverse();
    
        public static void AddAssertUnique<T>(this DynamicBuffer<T> buffer, T element)
            where T : unmanaged, IEquatable<T>
        {
            Debug.Assert(!buffer.Contains(element));
            buffer.Add(element);
        }

        public static void AddAssertUnique<T>(this NativeList<T> list, T element)
            where T : unmanaged, IEquatable<T>
        {
            Debug.Assert(!list.Contains(element));
            list.Add(element);
        }
    
        // public static unsafe void MemClear<T>(this NativeArray<T> array)
        //     where T : unmanaged
        // {
        //     var buffer = array.GetUnsafePtr();
        //     UnsafeUtility.MemClear(buffer, (long) array.Length * UnsafeUtility.SizeOf<T>());
        // }
        //
        // public static unsafe void MemClear<T>(this DynamicBuffer<T> buffer)
        //     where T : unmanaged => buffer.AsNativeArray().MemClear();

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void CheckIndexInRange(int value, int length)
        {
            if (value < 0)
            {
                throw new IndexOutOfRangeException($"Value {value} must be positive.");
            }

            if ((uint) value >= (uint) length)
            {
                throw new IndexOutOfRangeException($"Value {value} is out of range in NativeList of '{length}' Length.");
            }
        }
    }
}

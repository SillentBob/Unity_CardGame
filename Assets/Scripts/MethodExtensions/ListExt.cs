using System.Collections.Generic;
using UnityEngine;

namespace MethodExtensions
{
    public static class ListExt
    {
        public static void SafeSwap<T>(this List<T> list, int indexA, int indexB)
        {
            if (list == null)
            {
                Debug.LogError("List is null!");
                return;
            }

            if (indexA < 0 || indexA >= list.Count || indexB < 0 || indexB >= list.Count)
            {
                Debug.LogError($"One or more indices are out of range! A:{indexA} b:{indexB}");
                return;
            }

            if (indexA == indexB)
            {
                return;
            }

            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }
    }
}
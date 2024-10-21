using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities.Utils
{
    public static class RandomUtils
    {
        
        public delegate float WeightGetter<in TValue>(TValue value);
        public static bool RandomBool(float p = 0.5f) => Random.value < p;

        public static int RandomInt(int minInclusive, int maxInclusive)
        {
            return Mathf.RoundToInt(Random.Range(minInclusive, maxInclusive));
        }

        public static T RandomFromList<T>(List<T> list)
        {
            return list[RandomInt(0, list.Count - 1)];
        }

        public static T WeightedRandomFromList<T>(List<T> list, WeightGetter<T> weightGetter)
        {
            var totalWeight = list.Sum(i => weightGetter(i));
            var accumulatedWeight = 0f;
            var r = Random.Range(0, totalWeight);
            foreach (var item in list)
            {
                if (accumulatedWeight >= r)
                    return item;
                accumulatedWeight += weightGetter(item);
            }
            
            return RandomFromList(list);
        }
    }
}
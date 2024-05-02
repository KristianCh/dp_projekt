using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities.Utils
{
    public static class RandomUtils
    {
        public static bool RandomBool(float p = 0.5f) => Random.value < p;

        public static int RandomInt(int minInclusive, int maxInclusive)
        {
            return Mathf.RoundToInt(Random.Range(minInclusive, maxInclusive));
        }

        public static T RandomFromList<T>(List<T> list)
        {
            return list[RandomInt(0, list.Count - 1)];
        }
    }
}
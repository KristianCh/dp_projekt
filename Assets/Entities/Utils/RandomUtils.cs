// Author: Kristián Chovančák
// Created: 04.10.2023
// Copyright (c) Noxgames
// http://www.noxgames.com/

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities.Utils
{
    public static class RandomUtils
    {
        public static bool RandomBool(float p = 0.5f) => Random.value < p;
    }
}
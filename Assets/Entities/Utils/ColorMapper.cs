using System.Collections.Generic;
using UnityEngine;

namespace Entities.Utils
{
    /// <summary>
    /// Maps string to color for use in store.
    /// </summary>
    public static class ColorMapper
    {
        public static readonly Dictionary<string, Color> ColorMap = new Dictionary<string, Color>
        {
            {"black", Color.black},
            {"blue", Color.blue},
            {"cyan", Color.cyan},
            {"gray", Color.gray},
            {"green", Color.green},
            {"grey", Color.grey},
            {"magenta", Color.magenta},
            {"red", Color.red},
            {"white", Color.white},
            {"yellow", Color.yellow},
        };
    }
}
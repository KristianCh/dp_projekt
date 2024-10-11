using System.Collections.Generic;
using UnityEngine;

namespace Entities.DataManagement.Cosmetics
{
    [CreateAssetMenu(fileName = "StoreItemDefinitions", menuName = "StoreItemDefinitions", order = 0)]
    public class StoreItemDefinitions : ScriptableObject
    {
        [SerializeField]
        private List<StoreItem> _Items;
        
        public List<StoreItem> Items => _Items;
    }
}
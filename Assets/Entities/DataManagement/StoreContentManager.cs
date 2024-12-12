using System.Collections.Generic;
using System.Linq;
using Entities.DataManagement.Cosmetics;
using Entities.GameManagement;
using UnityEngine;

namespace Entities.DataManagement
{
    /// <summary>
    /// Manager handling getting store data.
    /// </summary>
    public class StoreContentManager : MonoBehaviour, IService
    {
        [SerializeField]
        private StoreItemDefinitions _StoreItemDefinitions;
        
        public List<StoreItem> Items => _StoreItemDefinitions.Items;

        public void Awake()
        {
            GameManager.AddService(this);
        }

        /// <summary>
        /// Gets StoreItem by code if it exists.
        /// </summary>
        public bool TryGetItemByItemCode(string itemCode, out StoreItem item)
        {
            item = _StoreItemDefinitions.Items.FirstOrDefault(i => i.ItemCode == itemCode);
            return item != null;
        }
    }
}
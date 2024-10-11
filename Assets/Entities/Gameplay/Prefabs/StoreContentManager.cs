using System;
using System.Collections.Generic;
using System.Linq;
using Entities.DataManagement.Cosmetics;
using Entities.GameManagement;
using UnityEngine;

namespace Entities.Gameplay.Prefabs
{
    public class StoreContentManager : MonoBehaviour, IService
    {
        [SerializeField]
        private StoreItemDefinitions _StoreItemDefinitions;
        
        public List<StoreItem> Items => _StoreItemDefinitions.Items;

        public void Awake()
        {
            GameManager.AddService(this);
        }

        public bool TryGetItemByItemCode(string itemCode, out StoreItem item)
        {
            item = _StoreItemDefinitions.Items.FirstOrDefault(i => i.ItemCode == itemCode);
            return item != null;
        }
    }
}
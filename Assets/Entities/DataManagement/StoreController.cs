using System;
using System.Collections.Generic;
using Entities.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Entities.DataManagement
{
    public class StoreController : PopupController
    {
        [Serializable]
        internal struct Item
        {
            [SerializeField]
            private string _Title;

            [SerializeField]
            private string _ColorCode;

            [SerializeField]
            private int _Cost;
            
            public string Title => _Title;
            public string ColorCode => _ColorCode;
            public int Cost => _Cost;
            public bool IsPurchased => PlayerPrefs.HasKey(_ColorCode) || _Cost == 0;
        }
        
        [FormerlySerializedAs("_Root")]
        [SerializeField]
        private RectTransform _ItemRootTransform;

        [SerializeField]
        private ScrollRect _ScrollRect;
        
        [SerializeField]
        private TMP_Text _CoinsText;
        
        [SerializeField]
        private StoreItem _StoreItemPrefab;
        
        [SerializeField]
        private List<Item> _Items;

        protected override void Awake()
        {
            base.Awake();

            foreach (var item in _Items)
            {
                var storeItem = Instantiate(_StoreItemPrefab, _ItemRootTransform);
                storeItem.Initialize(this, item, _ScrollRect.content.rect.width);
            }

            _ScrollRect.verticalNormalizedPosition = 0;
            UpdateCoins();
        }

        public void UpdateCoins()
        {
            _CoinsText.text = PlayerPrefs.GetInt("Coins").ToString();
        }
    }
}
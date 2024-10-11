using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.DataManagement.Cosmetics
{
    [Serializable]
    public enum ItemTypes
    {
        None = -1,
        PlayerColor = 0,
        Hat = 1,
        PlayerTexture = 2,
        ParticleEffect = 3,
    }
    
    [Serializable]
    public class StoreItem
    {
        [SerializeField]
        private string _Title;

        [FormerlySerializedAs("_ColorCode")] [SerializeField]
        private string _ItemCode;
            
        [SerializeField]
        private ItemTypes _ItemType;

        [SerializeField]
        private int _Cost;
            
        [SerializeField]
        [ShowIf(nameof(ShowIcon))]
        private Sprite _Icon;

        [SerializeField] [ShowIf(nameof(ShowPrefab))]
        private GameObject _ItemWorldPrefab;
        
        [SerializeField] [ShowIf(nameof(ShowTexture))]
        private Sprite _PlayerTexture;
            
        public string Title => _Title;
        public string ItemCode => _ItemCode;
        public ItemTypes ItemType => _ItemType;
        public int Cost => _Cost;
        public string StorageKeyValue => _ItemCode + _ItemType;
        public bool IsPurchased => PlayerPrefs.HasKey(StorageKeyValue) || _Cost == 0;
        public Sprite Icon => _Icon;
        public GameObject ItemWorldPrefab => _ItemWorldPrefab;
        public Sprite PlayerTexture => _PlayerTexture;

#if UNITY_EDITOR
        private bool ShowIcon => _ItemType != ItemTypes.PlayerColor && _ItemType != ItemTypes.PlayerTexture;
        private bool ShowPrefab => _ItemType == ItemTypes.Hat || _ItemType == ItemTypes.ParticleEffect;
        private bool ShowTexture => _ItemType == ItemTypes.PlayerTexture;
#endif
    }
}
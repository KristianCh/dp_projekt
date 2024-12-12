using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities.DataManagement.Cosmetics;
using Entities.GameManagement;
using Entities.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Entities.DataManagement
{
    /// <summary>
    /// Controller handling the store menu popup.
    /// </summary>
    public class StoreController : PopupController
    {
        [FormerlySerializedAs("_Root")]
        [SerializeField]
        private RectTransform _ItemRootTransform;

        [SerializeField]
        private ScrollRect _ScrollRect;
        
        [SerializeField]
        private TMP_Text _CoinsText;
        
        [Header("Cosmetics")]
        [SerializeField]
        private Button _ColorsButton;
        [SerializeField]
        private Button _HatsButton;
        [SerializeField]
        private Button _ParticlesButton;
        [SerializeField]
        private Button _TexturesButton;
        
        [FormerlySerializedAs("_StoreItemPrefab")] 
        [SerializeField]
        private StoreItemController storeItemControllerPrefab;
        
        private StoreContentManager storeContentManager;
        private ItemTypes currentItemType = ItemTypes.None;

        protected override void Awake()
        {
            base.Awake();
            storeContentManager = GameManager.GetService<StoreContentManager>();
            _ColorsButton.onClick.AddListener(() => PopulateContent(ItemTypes.PlayerColor));
            _HatsButton.onClick.AddListener(() => PopulateContent(ItemTypes.Hat));
            _ParticlesButton.onClick.AddListener(() => PopulateContent(ItemTypes.ParticleEffect));
            _TexturesButton.onClick.AddListener(() => PopulateContent(ItemTypes.PlayerTexture));
            
            PopulateContent(ItemTypes.PlayerColor);
            UpdateCoins();
        }

        /// <summary>
        /// Updates coin display.
        /// </summary>
        public void UpdateCoins()
        {
            _CoinsText.text = PlayerPrefs.GetInt("Coins").ToString();
        }
        
        /// <summary>
        /// Starts content population routine if not currently the displayed type.
        /// </summary>
        private void PopulateContent(ItemTypes type)
        {
            if (currentItemType == type) return;
            StartCoroutine(PopulateContentRoutine(type));
        }

        /// <summary>
        /// Clears current store content. Adds store content of requested type.
        /// </summary>
        private IEnumerator PopulateContentRoutine(ItemTypes type)
        {
            currentItemType = type;
            for(var i = _ScrollRect.content.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_ScrollRect.content.transform.GetChild(i).gameObject);
            }

            foreach (var item in storeContentManager.Items.Where(i => i.ItemType == type))
            {
                var storeItem = Instantiate(storeItemControllerPrefab, _ItemRootTransform);
                storeItem.Initialize(this, item, _ScrollRect.content.rect.width);
            }
            yield return null;
            _ScrollRect.verticalNormalizedPosition = 1;
        }
    }
}
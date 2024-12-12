using System;
using DG.Tweening;
using Entities.DataManagement.Cosmetics;
using Entities.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.DataManagement
{
    /// <summary>
    /// Controller for individual store items.
    /// </summary>
    public class StoreItemController : MonoBehaviour
    {
        [SerializeField]
        private Image _Image;

        [SerializeField]
        private TMP_Text _TitleText;
        
        [SerializeField]
        private TMP_Text _CostText;
        
        [SerializeField]
        private Button _Button;

        [SerializeField]
        private RectTransform _UnpurchasedPanel;

        [SerializeField]
        private RectTransform _PurchasedPanel;
        
        [SerializeField]
        private Image _EquippedImage;

        private StoreController _storeController;
        private StoreItem _item;

        private static event Action ItemEquipped;

        /// <summary>
        /// Sets up displayed content, size of item and button listener.
        /// </summary>
        internal void Initialize(StoreController storeController, StoreItem item, float width)
        {
            _storeController = storeController;
            _item = item;
            _TitleText.text = _item.Title;

            switch (_item.ItemType)
            {
                case ItemTypes.PlayerColor:
                    _Image.color = ColorMapper.ColorMap[item.ItemCode];
                    break;
                case ItemTypes.Hat:
                case ItemTypes.ParticleEffect:
                    _Image.sprite = _item.Icon;
                    break;
                case ItemTypes.PlayerTexture:
                    _Image.sprite = _item.PlayerTexture;
                    break;
            }
            
            
            _CostText.text = _item.Cost.ToString();
            ItemEquipped += OnItemEquipped;
            _Button.onClick.AddListener(OnButtonClick);
            
            _UnpurchasedPanel.gameObject.SetActive(!_item.IsPurchased);
            _PurchasedPanel.gameObject.SetActive(_item.IsPurchased);
            _EquippedImage.gameObject.SetActive(_item.ItemCode == PlayerPrefs.GetString(_item.ItemType.ToString()));
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 1.5f);
        }

        private void OnDestroy()
        {
            ItemEquipped -= OnItemEquipped;
        }

        /// <summary>
        /// Handles button click. If owned, equips item. If not owned and player has enough coins, purchases item and equps it. Otherwise animates to indicate not enough coins.
        /// </summary>
        private void OnButtonClick()
        {
            if (!_item.IsPurchased)
            {
                var currentCoins = PlayerPrefs.GetInt("Coins");
                if (currentCoins >= _item.Cost)
                {
                    transform.DOLocalJump(transform.localPosition, 10, 1, 0.5f);
                    PlayerPrefs.SetInt("Coins", currentCoins - _item.Cost);
                    PlayerPrefs.SetInt(_item.StorageKeyValue, 1);
                    PlayerPrefs.Save();

                    _storeController.UpdateCoins();
                }
                else
                {
                    transform.DOShakeRotation(0.1f, 10f);
                    return;
                }
            }
            
            if (_item.IsPurchased)
            {
                ItemEquipped?.Invoke();
                _EquippedImage.gameObject.SetActive(true);
                PlayerPrefs.SetString(_item.ItemType.ToString(), _item.ItemCode);
                PlayerPrefs.Save();
            }
            
            _UnpurchasedPanel.gameObject.SetActive(!_item.IsPurchased);
            _PurchasedPanel.gameObject.SetActive(_item.IsPurchased);
        }

        /// <summary>
        /// Updates display to equiped state.
        /// </summary>
        private void OnItemEquipped()
        {
            if (_EquippedImage == null) return;
            _EquippedImage.gameObject.SetActive(false);
        }
    }
}
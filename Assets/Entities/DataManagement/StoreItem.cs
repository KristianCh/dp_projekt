using System;
using DG.Tweening;
using Entities.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.DataManagement
{
    public class StoreItem : MonoBehaviour
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
        private StoreController.Item _item;

        private static event Action ItemEquipped;

        internal void Initialize(StoreController storeController, StoreController.Item item, float width)
        {
            _storeController = storeController;
            _item = item;
            _TitleText.text = _item.Title;
            _Image.color = ColorMapper.ColorMap[item.ColorCode];
            _CostText.text = _item.Cost.ToString();
            ItemEquipped += OnItemEquipped;
            _Button.onClick.AddListener(OnButtonClick);
            
            _UnpurchasedPanel.gameObject.SetActive(!_item.IsPurchased);
            _PurchasedPanel.gameObject.SetActive(_item.IsPurchased);
            _EquippedImage.gameObject.SetActive(_item.ColorCode == PlayerPrefs.GetString("Color"));
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 1.5f);
        }

        private void OnDestroy()
        {
            ItemEquipped -= OnItemEquipped;
        }

        private void OnButtonClick()
        {
            if (!_item.IsPurchased)
            {
                var currentCoins = PlayerPrefs.GetInt("Coins");
                if (currentCoins >= _item.Cost)
                {
                    transform.DOLocalJump(transform.localPosition, 10, 1, 0.5f);
                    PlayerPrefs.SetInt("Coins", currentCoins - _item.Cost);
                    PlayerPrefs.SetInt(_item.ColorCode, 1);
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
                PlayerPrefs.SetString("Color", _item.ColorCode);
                PlayerPrefs.Save();
            }
            
            _UnpurchasedPanel.gameObject.SetActive(!_item.IsPurchased);
            _PurchasedPanel.gameObject.SetActive(_item.IsPurchased);
        }

        private void OnItemEquipped()
        {
            if (_EquippedImage == null) return;
            _EquippedImage.gameObject.SetActive(false);
        }
    }
}
using System;
using DG.Tweening;
using Entities.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.Utils
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _Root;

        [SerializeField]
        private Button _CancelButton;
        
        public Signal OnPopupOpening = new();
        public Signal OnPopupOpened = new();
        public Signal OnPopupClosing = new();
        public Signal OnPopupClosed = new();

        protected virtual void Awake()
        {
            _CancelButton.onClick.AddListener(ClosePopup);
        }

        protected virtual void Start()
        {
            AnimateIn();
        }

        public void SetCancelable(bool cancelable)
        {
            _CancelButton.gameObject.SetActive(cancelable);
        }

        protected virtual void ClosePopup()
        {
            _CancelButton.onClick.RemoveAllListeners();
            OnPopupClosed.AddOnce(() => Destroy(gameObject));
            AnimateOut();
        }

        protected virtual void AnimateIn()
        {
            OnPopupOpening.Dispatch();
            _Root.localScale = Vector3.zero;
            _Root.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() => OnPopupOpened.Dispatch());
        }

        protected virtual void AnimateOut()
        {
            OnPopupClosing.Dispatch();
            _Root.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => OnPopupClosed.Dispatch());
        }
    }
}
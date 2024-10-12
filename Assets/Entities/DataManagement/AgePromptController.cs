using DG.Tweening;
using Entities.Database;
using Entities.GameManagement;
using Entities.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.DataManagement
{
    public class AgePromptController : PopupController
    {

        [SerializeField]
        private TMP_InputField _AgeInputField;

        [SerializeField]
        private TMP_InputField _NickInputField;

        [SerializeField]
        private Button _SubmitButton;

        private DatabaseHandler _databaseHandler;
        private PlayerDataManager _playerDataManager;

        protected override void Awake()
        {
            base.Awake();
            
            _databaseHandler = GameManager.GetService<DatabaseHandler>();
            _playerDataManager = GameManager.GetService<PlayerDataManager>();
            
            _SubmitButton.onClick.AddListener(OnSubmit);
            _AgeInputField.text = _playerDataManager.PlayerAge.ToString();
            _NickInputField.text = _playerDataManager.PlayerNickname;
        }
        
        protected override void ClosePopup()
        {
            base.ClosePopup();
            _SubmitButton.onClick.RemoveAllListeners();
        }

        private void OnSubmit()
        {
            if (int.TryParse(_AgeInputField.text, out var value) && value <= 100 && value >= 3)
            {
                _playerDataManager.PlayerAge = value;
                if (_NickInputField.text != _playerDataManager.PlayerNickname)
                {
                    _playerDataManager.PlayerNickname = _NickInputField.text;
                    _databaseHandler.RecordPlayerData();
                }
                
                ClosePopup();
            }
        }
    }
}
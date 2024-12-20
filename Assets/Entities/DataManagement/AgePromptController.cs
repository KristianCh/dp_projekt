﻿using DG.Tweening;
using Entities.Database;
using Entities.GameManagement;
using Entities.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.DataManagement
{
    /// <summary>
    /// Controller handling the age prompt menu popup.
    /// </summary>
    public class AgePromptController : PopupController
    {

        [SerializeField]
        private TMP_InputField _AgeInputField;

        [SerializeField]
        private TMP_InputField _NickInputField;

        [SerializeField]
        private Button _SubmitButton;
        
        [SerializeField]
        private TMP_Text _ErrorText;

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
            _ErrorText.gameObject.SetActive(false);
        }
        
        protected override void ClosePopup()
        {
            base.ClosePopup();
            _SubmitButton.onClick.RemoveAllListeners();
        }
        
        /// <summary>
        /// Handles checking data for correct values and displaying an error message in case it is incorrect. Calls database handler to update player data.
        /// </summary>
        private void OnSubmit()
        {
            if (int.TryParse(_AgeInputField.text, out var value) && value <= 100 && value >= 1)
            {
                _playerDataManager.PlayerAge = value;
                if (_NickInputField.text != _playerDataManager.PlayerNickname)
                {
                    _playerDataManager.PlayerNickname = _NickInputField.text;
                    _databaseHandler.RecordPlayerData();
                    _ErrorText.gameObject.SetActive(false);
                }
                ClosePopup();
            }
            else 
                DisplayError();
        }
        
        /// <summary>
        /// Shows and animates error message.
        /// </summary>
        private void DisplayError()
        {
            _ErrorText.gameObject.SetActive(true);
            _ErrorText.transform.DOShakeRotation(0.1f, 10f);
        }
    }
}
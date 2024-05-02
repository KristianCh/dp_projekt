using DG.Tweening;
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

        protected override void Awake()
        {
            base.Awake();
            
            _databaseHandler = GameManager.GetService<DatabaseHandler>();
            _SubmitButton.onClick.AddListener(OnSubmit);
            _AgeInputField.text = PlayerPrefs.GetInt("Age").ToString();
            _NickInputField.text = PlayerPrefs.GetString("Nickname");
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
                PlayerPrefs.SetInt("Age", value);
                if (_NickInputField.text != PlayerPrefs.GetString("Nickname"))
                {
                    PlayerPrefs.SetString("Nickname", _NickInputField.text);
                    _databaseHandler.RecordPlayerData();
                }
                
                PlayerPrefs.Save();
                ClosePopup();
            }
        }
    }
}
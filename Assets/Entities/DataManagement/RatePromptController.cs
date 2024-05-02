using DG.Tweening;
using Entities.GameManagement;
using Entities.Utils;
using Entities.WordProcessing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.DataManagement
{
    public class RatePromptController : PopupController
    {
        [Header("RatePanel")]
        [SerializeField]
        private RectTransform _RatePanel;
        
        [SerializeField]
        private TMP_Text _WordDisplay;
        
        [SerializeField]
        private TMP_InputField _InputField;

        [SerializeField]
        private Button _SubmitButton;
        
        [Header("ThankYouPanel")]
        [SerializeField]
        private RectTransform _ThankYouPanel;

        [SerializeField]
        private Button _RateAnotherButton;

        [SerializeField]
        private int rateReward = 5;
        
        private WordTriple _worldTriple;
        
        private DatabaseHandler _databaseHandler;
        private WordProcessingManager _wordProcessingManager;

        protected override void Awake()
        {
            base.Awake();
            _SubmitButton.onClick.AddListener(OnSubmit);
            _RateAnotherButton.onClick.AddListener(OnRateAnother);
            
            _databaseHandler = GameManager.GetService<DatabaseHandler>();
            _wordProcessingManager = GameManager.GetService<WordProcessingManager>();
            
            SetWord();
        }
        
        protected override void ClosePopup()
        {
            base.ClosePopup();
            _SubmitButton.onClick.RemoveAllListeners();
        }

        private void SetWord()
        {
            _worldTriple = _wordProcessingManager.GetWordTriple();
            _WordDisplay.text = _wordProcessingManager.NicifyWord(_worldTriple.MainWord);
        }

        private void OnSubmit()
        {
            if (int.TryParse(_InputField.text, out var value) && value <= 100)
            {
                _databaseHandler.RecordManualRating(_worldTriple.MainWord, _worldTriple.WordAOA, value);
				
				var currentCoins = PlayerPrefs.GetInt("Coins");
				PlayerPrefs.SetInt("Coins", currentCoins + rateReward);
                    
                _RatePanel.gameObject.SetActive(false);
                _ThankYouPanel.gameObject.SetActive(true);
                _InputField.text = "";
            }
        }

        private void OnRateAnother()
        {
			SetWord();
			
            _RatePanel.gameObject.SetActive(true);
            _ThankYouPanel.gameObject.SetActive(false);
        }
    }
}
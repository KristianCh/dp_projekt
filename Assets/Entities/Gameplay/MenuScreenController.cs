using System;
using Entities.DataManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Entities.Gameplay
{
    public class MenuScreenController : MonoBehaviour
    {
        [FormerlySerializedAs("_Button")]
        [SerializeField]
        private Button _StartGameButton;
        
        [SerializeField]
        private Button _SetAgeButton;
        
        [SerializeField]
        private Button _ManualRateButton;
        
        [SerializeField]
        private Button _StoreButton;
        
        [SerializeField]
        private Button _HighscoreButton;
        
        [SerializeField]
        private AgePromptController _AgePromptPrefab;
        
        [SerializeField]
        private RatePromptController _RatePromptPrefab;
        
        [SerializeField]
        private StoreController _StorePrefab;
        
        [SerializeField]
        private HighscorePanelController _HighscorePrefab;
        
        [SerializeField]
        private TMP_Text _HighScoreText;
        
        private bool _isPromptOpen = false;

        public void Awake()
        {
            _StartGameButton.onClick.AddListener(StartGame);
            _SetAgeButton.onClick.AddListener(ShowAgePrompt);
            _ManualRateButton.onClick.AddListener(ShowRatePrompt);
            _StoreButton.onClick.AddListener(ShowStore);
            _HighscoreButton.onClick.AddListener(ShowLeaderboard);

            _HighScoreText.text = "Vysoké Skóre: " + Mathf.RoundToInt(PlayerPrefs.GetFloat("HighScore"));
        }

        public void Start()
        {
            if (!PlayerPrefs.HasKey("Age"))
                ShowAgePrompt(false);
            else
                UpdateAgeDisplay();
        }

        private void StartGame()
        {
            SceneManager.LoadScene("MainScene");
        }

        private void ShowAgePrompt() => ShowAgePrompt(true);

        private void ShowAgePrompt(bool cancelable)
        { 
            if (_isPromptOpen) return;
            var prompt = Instantiate(_AgePromptPrefab, transform);
            _isPromptOpen = true;
            prompt.SetCancelable(cancelable);
            
            prompt.OnPopupClosing.AddOnce(UpdateAgeDisplay);
            prompt.OnPopupClosed.AddOnce(() => _isPromptOpen = false);
        }

        private void ShowRatePrompt()
        { 
            if (_isPromptOpen) return;
            var prompt = Instantiate(_RatePromptPrefab, transform);
            _isPromptOpen = true;
            
            prompt.OnPopupClosed.AddOnce(() => _isPromptOpen = false);
        }

        private void ShowStore()
        {
            if (_isPromptOpen) return;
            var prompt = Instantiate(_StorePrefab, transform);
            _isPromptOpen = true;
            
            prompt.OnPopupClosed.AddOnce(() => _isPromptOpen = false);
        }

        private void ShowLeaderboard()
        {
            if (_isPromptOpen) return;
            var prompt = Instantiate(_HighscorePrefab, transform);
            _isPromptOpen = true;
            
            prompt.OnPopupClosed.AddOnce(() => _isPromptOpen = false);
        }

        private void UpdateAgeDisplay()
        {
            var text = _SetAgeButton.GetComponentInChildren<TMP_Text>();
            text.text = "Vek " + (PlayerPrefs.HasKey("Age") ? PlayerPrefs.GetInt("Age") : "??");
        }
    }
}
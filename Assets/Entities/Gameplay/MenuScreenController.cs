using System;
using Entities.DataManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Entities.Gameplay
{
    /// <summary>
    /// Controller handling button presses in the main menu.
    /// </summary>
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

        /// <summary>
        /// Opens unclosable age prompt if the age has not been set yet.
        /// </summary>
        public void Start()
        {
            if (!PlayerPrefs.HasKey("Age"))
                ShowAgePrompt(false);
            else
                UpdateAgeDisplay();
        }

        /// <summary>
        /// Starts game.
        /// </summary>
        private void StartGame()
        {
            SceneManager.LoadScene("MainScene");
        }
        
        /// <summary>
        /// Opens age prompt from button press.
        /// </summary>
        private void ShowAgePrompt() => ShowAgePrompt(true);

        /// <summary>
        /// Opens age prompt.
        /// </summary>
        private void ShowAgePrompt(bool cancelable)
        { 
            if (_isPromptOpen) return;
            var prompt = Instantiate(_AgePromptPrefab, transform);
            _isPromptOpen = true;
            prompt.SetCancelable(cancelable);
            
            prompt.OnPopupClosing.AddOnce(UpdateAgeDisplay);
            prompt.OnPopupClosed.AddOnce(() => _isPromptOpen = false);
        }

        /// <summary>
        /// Opens manual rate popup.
        /// </summary>
        private void ShowRatePrompt()
        { 
            if (_isPromptOpen) return;
            var prompt = Instantiate(_RatePromptPrefab, transform);
            _isPromptOpen = true;
            
            prompt.OnPopupClosed.AddOnce(() => _isPromptOpen = false);
        }

        /// <summary>
        /// Opens store popup.
        /// </summary>
        private void ShowStore()
        {
            if (_isPromptOpen) return;
            var prompt = Instantiate(_StorePrefab, transform);
            _isPromptOpen = true;
            
            prompt.OnPopupClosed.AddOnce(() => _isPromptOpen = false);
        }

        /// <summary>
        /// Opens leaderboard popup.
        /// </summary>
        private void ShowLeaderboard()
        {
            if (_isPromptOpen) return;
            var prompt = Instantiate(_HighscorePrefab, transform);
            _isPromptOpen = true;
            
            prompt.OnPopupClosed.AddOnce(() => _isPromptOpen = false);
        }

        /// <summary>
        /// Updates player age and nickname in display.
        /// </summary>
        private void UpdateAgeDisplay()
        {
            var text = _SetAgeButton.GetComponentInChildren<TMP_Text>();
            var nick = PlayerPrefs.GetString("Nickname");
            var playerPk = PlayerPrefs.GetInt("PlayerPK");
            if (string.IsNullOrEmpty(nick)) nick = "Hráč" + playerPk;
            text.text = "Vek " + PlayerPrefs.GetInt("Age") + "\n" + nick;
        }
    }
}
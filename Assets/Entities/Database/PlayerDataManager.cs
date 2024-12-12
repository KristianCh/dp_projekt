using Entities.Events;
using Entities.GameManagement;
using UnityEngine;

namespace Entities.Database
{
    /// <summary>
    /// Handles updating and saving player data to PlayerPrefs and dispatching signals on data updates.
    /// </summary>
    public class PlayerDataManager : MonoBehaviour, IService
    {
        public Signal<string> PlayerGuidChanged = new();
        public Signal<int> PlayerPkChanged = new();
        public Signal<string> PlayerNicknameChanged = new();
        public Signal<int> PlayerAgeChanged = new();
        public Signal<float> PlayerHighscoreChanged = new();
        
        private string _playerGuid;
        private int _playerPk;
        private string _playerNickname;
        private int _playerAge;
        private float _playerHighscore;
        
        private bool _hasPlayerGuidChanged;
        private bool _hasPlayerPkChanged;
        private bool _hasPlayerNicknameChanged;
        private bool _hasPlayerAgeChanged;
        private bool _hasPlayerHighscoreChanged;

        public string PlayerGuid
        {
            get => _playerGuid;
            set
            {
                _playerGuid = value;
                _hasPlayerGuidChanged = true;
                PlayerGuidChanged.Dispatch(value);
            }
        }
        
        public int PlayerPk
        {
            get => _playerPk;
            set
            {
                _playerPk = value;
                _hasPlayerPkChanged = true;
                PlayerPkChanged.Dispatch(value);
            }
        }

        public string PlayerNickname
        {
            get => _playerNickname;
            set
            {
                _playerNickname = value;
                _hasPlayerNicknameChanged = true;
                PlayerNicknameChanged.Dispatch(value);
            }
        }

        public int PlayerAge
        {
            get => _playerAge;
            set
            {
                _playerAge = value;
                _hasPlayerAgeChanged = true;
                PlayerAgeChanged.Dispatch(value);
            }
        }

        public float PlayerHighscore
        {
            get => _playerHighscore;
            set
            {
                _playerHighscore = value;
                _hasPlayerHighscoreChanged = true;
                PlayerHighscoreChanged.Dispatch(value);
            }
        }

        private void Awake()
        {
            GameManager.AddService(this);
            PlayerGuid = PlayerPrefs.GetString("PlayerGUID");
            PlayerPk = PlayerPrefs.GetInt("PlayerPK");
            PlayerNickname = PlayerPrefs.GetString("Nickname");
            PlayerHighscore = PlayerPrefs.GetFloat("HighScore");
            PlayerAge = PlayerPrefs.GetInt("Age");
        }
        
        /// <summary>
        /// Player data is often changed from Tasks, from which we cannot call PlayerPrefs functions. Checks wether any have changed and saves them.
        /// </summary>
        private void Update()
        {
            if (_hasPlayerGuidChanged)
            {
                _hasPlayerGuidChanged = false;
                PlayerPrefs.SetString("PlayerGUID", PlayerGuid);
                PlayerPrefs.Save();
            }

            if (_hasPlayerPkChanged)
            {
                _hasPlayerPkChanged = false;
                PlayerPrefs.SetInt("PlayerPK", PlayerPk);
                PlayerPrefs.Save();
            }

            if (_hasPlayerNicknameChanged)
            {
                _hasPlayerNicknameChanged = false;
                PlayerPrefs.SetString("Nickname", PlayerNickname);
                PlayerPrefs.Save();
            }

            if (_hasPlayerAgeChanged)
            {
                _hasPlayerAgeChanged = false;
                PlayerPrefs.SetInt("Age", PlayerAge);
                PlayerPrefs.Save();
            }

            if (_hasPlayerHighscoreChanged)
            {
                _hasPlayerHighscoreChanged = false;
                PlayerPrefs.SetFloat("HighScore", PlayerHighscore);
                PlayerPrefs.Save();
            }
        }
    }
}
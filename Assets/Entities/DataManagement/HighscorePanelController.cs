using System.Collections.Generic;
using Entities.GameManagement;
using Entities.Utils;
using TMPro;
using UnityEngine;

namespace Entities.DataManagement
{
    public class HighscorePanelController : PopupController
    {
        [SerializeField]
        private TMP_Text _HighscoreText;
        
        [SerializeField]
        private List<HighscoreItem> _HighscoreItems = new ();

        private DatabaseHandler _databaseHandler;

        protected override void Awake()
        {
            base.Awake();

            _databaseHandler = GameManager.GetService<DatabaseHandler>();
            _HighscoreText.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("HighScore")).ToString();

            var players = _databaseHandler.GetTopPlayers();
            for (var i = 0; i < _HighscoreItems.Count; i++)
            {
                if (i < players.Count)
                {
                    _HighscoreItems[i].SetData(i + 1, players[i].nick, players[i].highscore, players[i].guid == PlayerPrefs.GetString("PlayerGUID"));
                }
                else
                    _HighscoreItems[i].gameObject.SetActive(false);
            }
        }
    }
}
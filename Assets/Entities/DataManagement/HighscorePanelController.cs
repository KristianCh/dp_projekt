﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Entities.GameManagement;
using Entities.Utils;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Entities.DataManagement
{
    /// <summary>
    /// Controller handling the highscore menu popup.
    /// </summary>
    public class HighscorePanelController : PopupController
    {
        [SerializeField]
        private TMP_Text _HighscoreText;
        
        [SerializeField]
        private GameObject _HighscoreItemsContainer;
        
        [SerializeField]
        private List<HighscoreItem> _HighscoreItems = new ();

        private DatabaseHandler _databaseHandler;
        private List<(string guid, string nick, float highscore)> _players;
        private Coroutine _setScoresCoroutine;
        private bool _hadErrorLoading = false;

        protected override void Awake()
        {
            base.Awake();

            _databaseHandler = GameManager.GetService<DatabaseHandler>();
            _setScoresCoroutine = StartCoroutine(SetHighscores());
            Task.Run(LoadHighscores);
        }

        /// <summary>
        /// Requests top players list from database handler. Updates player list on success, if setting scores routine is running triggers error.
        /// </summary>
        private async UniTask LoadHighscores()
        {
            try
            {
                _players = await _databaseHandler.GetTopPlayers();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                if (_setScoresCoroutine != null)
                {
                    _hadErrorLoading = true;
                }
                
                throw;
            }
        }

        /// <summary>
        /// Waits until highscores are loaded from the database or until there's an error response. Updates highscore displays on success, displays error message on error.
        /// </summary>
        private IEnumerator SetHighscores()
        {
            _HighscoreText.text = "Načítavanie...";
            _HighscoreItemsContainer.SetActive(false);
            
            while (_players.IsNullOrEmpty())
            {
                if (_hadErrorLoading)
                {
                    _HighscoreText.text = "Problém pri načítaní vysokého skóre";
                    yield break;
                }
                yield return null;
            }
            
            for (var i = 0; i < _HighscoreItems.Count; i++)
            {
                if (i < _players.Count)
                {
                    _HighscoreItems[i].SetData(i + 1, _players[i].nick, _players[i].highscore, _players[i].guid == PlayerPrefs.GetString("PlayerGUID"));
                }
                else
                    _HighscoreItems[i].gameObject.SetActive(false);
            }
            _HighscoreItemsContainer.SetActive(true);
            _HighscoreText.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("HighScore")).ToString();
        }
    }
}
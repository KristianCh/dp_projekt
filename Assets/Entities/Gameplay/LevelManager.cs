using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Entities.Database;
using Entities.GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Entities.Gameplay
{
    /// <summary>
    /// Controls spawning of objects in game and speed of the game. Reacts to playing interacting with coins, obstacles and rating gates.
    /// </summary>
    public class LevelManager : MonoBehaviour, IService
    {
        [SerializeField]
        private LevelDefinition _LevelDefinition;
        
        [SerializeField]
        private List<GameObject> _WorldObstacles = new();
        
        [SerializeField]
        private List<GameObject> _WorldItems = new();

        [SerializeField]
        private List<Transform> _TrackRoots = new();

        [SerializeField]
        private WordTripleGate _GatePrefab;

        [SerializeField]
        private GameObject _PlayerObject;

        [Header("UI")]
        [SerializeField]
        private TMP_Text _ScoreValueText;
        
        [SerializeField]
        private TMP_Text _ComboValueText;
        
        [SerializeField]
        private TMP_Text _CoinsValueText;

        [SerializeField]
        private RectTransform _GameOverDisplay;
        
        private List<Coroutine> _runningSpawnRoutines = new();
        
        private float _score = 0;
        private float _combo = 0;
        private int _coins = 0;
        private int _lastTrackIndex = 0;
        private bool _isDead;

        private float _obstacleSpawnFrequency = 4;
        private float _itemSpawnFrequency = 5;
        private float _speedMultiplier = 1;
        private float _targetSpeedMultiplier = 1;
        private Tween _speedMultiplierIncreaseTween;
        private DatabaseHandler _databaseHandler;
        private PlayerDataManager _playerDataManager;

        public float SpeedMultiplier => _speedMultiplier;
        public GameObject PlayerObject => _PlayerObject;

        
        /// <summary>
        /// Initializes object spawning.
        /// </summary>
        private void Start()
        {
            GameManager.AddService(this);
            _playerDataManager = GameManager.GetService<PlayerDataManager>();
            _isDead = false;
            
            if (TryGetObstacleSpawnConfig(out var spawnConfig))
            {
                ScheduleObstacleSpawn(1, spawnConfig.objectToSpawn, spawnConfig.parentTrack);
            }
            if (TryGetItemSpawnConfig(out spawnConfig))
            {
                ScheduleItemSpawn(_itemSpawnFrequency, spawnConfig.objectToSpawn, spawnConfig.parentTrack);
            }

            StartCoroutine(GateSpawnRoutine());

            _speedMultiplier = _LevelDefinition.StartSpeed;
            _speedMultiplierIncreaseTween = DOTween.To(
            () => (float)_targetSpeedMultiplier, 
            x => _targetSpeedMultiplier = (float)x, 
            (float)_LevelDefinition.MaxSpeed, 
            (float)_LevelDefinition.SpeedupEndTime.timeSpan.TotalSeconds - (float)_LevelDefinition.SpeedupStartTime.timeSpan.TotalSeconds
            ).SetDelay((float)_LevelDefinition.SpeedupStartTime.timeSpan.TotalSeconds);
        }

        private void Awake()
        {
            _databaseHandler = GameManager.GetService<DatabaseHandler>();
        }

        /// <summary>
        /// Updates score.
        /// </summary>
        public void Update()
        {
            _speedMultiplier = Mathf.Lerp(_speedMultiplier, _targetSpeedMultiplier, Time.deltaTime);
            if (_isDead) return;
            _score += _speedMultiplier * Time.deltaTime * (1 + _combo);
            _ScoreValueText.text = Mathf.RoundToInt(_score).ToString();
        }

        private void OnDestroy()
        {
            GameManager.RemoveService<LevelManager>();
        }

        /// <summary>
        /// Increases combo on correct rating.
        /// </summary>
        public void IncrementCombo()
        {
            _combo++;
            _ComboValueText.text = Mathf.RoundToInt(_combo).ToString();
        }

        /// <summary>
        /// Decreases combo on neutral rating.
        /// </summary>
        public void DecrementCombo()
        {
            _combo = Mathf.Max(0, _combo - 1);
            _ComboValueText.text = Mathf.RoundToInt(_combo).ToString();
        }

        /// <summary>
        /// Set combo to 0 on incorrect rating.
        /// </summary>
        public void ResetCombo()
        {
            _combo = 0;
            _ComboValueText.text = Mathf.RoundToInt(_combo).ToString();
        }

        /// <summary>
        /// Increases score and coins on coin pickup.
        /// </summary>
        public void OnCoinsPickedUp()
        {
            _score += 100;
            _coins++;
            _CoinsValueText.text = Mathf.RoundToInt(_coins).ToString();
        }
        
        /// <summary>
        /// Handles player colliding with an obstacles. Changes back to menu scene.
        /// </summary>
        public void OnDeath()
        {
            _speedMultiplierIncreaseTween?.Kill();
            _targetSpeedMultiplier = 0;
            _isDead = true;
            _GameOverDisplay.gameObject.SetActive(true);
            
            UpdatePlayerDataOnGameOver(_score, _coins);
            
            StartCoroutine(ChangeToGameOverScreen());

            IEnumerator ChangeToGameOverScreen()
            {
                yield return new WaitForSeconds(3f);
                SceneManager.LoadScene("MenuScene");
            }
        }

        /// <summary>
        /// Prepares to spawn next obstacle.
        /// </summary>
        private void OnObstacleSpawned()
        {
            _runningSpawnRoutines.Remove(null);
            if (TryGetObstacleSpawnConfig(out var spawnConfig))
            {
                ScheduleObstacleSpawn(_obstacleSpawnFrequency / _speedMultiplier, spawnConfig.objectToSpawn, spawnConfig.parentTrack);
            }
        }

        /// <summary>
        /// Prepares to spawn next item.
        /// </summary>
        private void OnItemSpawned()
        {
            _runningSpawnRoutines.Remove(null);
            if (TryGetItemSpawnConfig(out var spawnConfig))
            {
                ScheduleItemSpawn(_itemSpawnFrequency / _speedMultiplier, spawnConfig.objectToSpawn, spawnConfig.parentTrack);
            }
        }

        /// <summary>
        /// Schedules next obstacle spawn.
        /// </summary>
        private void ScheduleObstacleSpawn(float duration, GameObject objectToSpawn, Transform parentTrack)
        {
            var coroutine = StartCoroutine(ObstacleSpawnRoutine(duration, objectToSpawn, parentTrack));
            _runningSpawnRoutines.Add(coroutine);
        }

        /// <summary>
        /// Schedules next item spawn.
        /// </summary>
        private void ScheduleItemSpawn(float duration, GameObject objectToSpawn, Transform parentTrack)
        {
            var coroutine = StartCoroutine(ItemSpawnRoutine(duration, objectToSpawn, parentTrack));
            _runningSpawnRoutines.Add(coroutine);
        }

        /// <summary>
        /// Spawns and animates in new obstacle.
        /// </summary>
        private IEnumerator ObstacleSpawnRoutine(float duration, GameObject objectToSpawn, Transform parentTrack)
        {
            yield return new WaitForSeconds(duration);
            if (_isDead) yield break;
            var spawnedObject = Instantiate(objectToSpawn, parentTrack);
            spawnedObject.transform.localPosition = new Vector3(0, 0, 50);
            OnObstacleSpawned();
        }

        /// <summary>
        /// Spawns and animates in new item.
        /// </summary>
        private IEnumerator ItemSpawnRoutine(float duration, GameObject objectToSpawn, Transform parentTrack)
        {
            yield return new WaitForSeconds(duration);
            if (_isDead) yield break;
            var spawnedObject = Instantiate(objectToSpawn, parentTrack);
            spawnedObject.transform.localPosition = new Vector3(0, 0, 50);
            OnItemSpawned();
        }

        /// <summary>
        /// Spawns and animates in new gate.
        /// </summary>
        private IEnumerator GateSpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                if (_isDead) yield break;
                var gate = Instantiate(_GatePrefab);
                gate.transform.localPosition = new Vector3(0, 0, 50 * _speedMultiplier);
            }
        }

        /// <summary>
        /// Prepares setting for next obstacle spawn. Picks track to spawn in and prefab to spawn.
        /// </summary>
        private bool TryGetObstacleSpawnConfig(out (GameObject objectToSpawn, Transform parentTrack) data)
        {
            data = (null, null);
            if (_WorldObstacles.Count == 0 || _TrackRoots.Count == 0) return false;
            data.objectToSpawn = _WorldObstacles[Random.Range(0, _WorldObstacles.Count)];
            var trackIndex = Random.Range(0, _TrackRoots.Count);
            while (trackIndex == _lastTrackIndex)
                trackIndex = Random.Range(0, _TrackRoots.Count);
            data.parentTrack = _TrackRoots[trackIndex];
            _lastTrackIndex = trackIndex;
            return true;
        }
        
        /// <summary>
        /// Prepares setting for next item spawn. Picks track to spawn in and prefab to spawn.
        /// </summary>
        private bool TryGetItemSpawnConfig(out (GameObject objectToSpawn, Transform parentTrack) data)
        {
            data = (null, null);
            if (_WorldItems.Count == 0 || _TrackRoots.Count == 0) return false;
            data.objectToSpawn = _WorldItems[Random.Range(0, _WorldItems.Count)];
            var trackIndex = Random.Range(0, _TrackRoots.Count);
            while (trackIndex == _lastTrackIndex)
                trackIndex = Random.Range(0, _TrackRoots.Count);
            data.parentTrack = _TrackRoots[trackIndex];
            _lastTrackIndex = trackIndex;
            return true;
        }

        /// <summary>
        /// Checks for new highscore, saves and updates database. Saves coins earned.
        /// </summary>
        private void UpdatePlayerDataOnGameOver(float finalScore, int collectedCoins)
        {
            if (_playerDataManager.PlayerHighscore < finalScore)
            {
                _playerDataManager.PlayerHighscore = finalScore;
                
                _databaseHandler.RecordPlayerData();
            }

            var totalCoins = 0;
            if (PlayerPrefs.HasKey("Coins")) totalCoins = PlayerPrefs.GetInt("Coins");
            PlayerPrefs.SetInt("Coins", totalCoins + collectedCoins);
            PlayerPrefs.Save();
        }
    }
}
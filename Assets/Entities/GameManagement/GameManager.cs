using System;
using System.Collections;
using System.Collections.Generic;
using Entities.DataManagement.Cosmetics;
using Entities.Events;
using Unity.VisualScripting;
using UnityEngine;

namespace Entities.GameManagement
{
    /// <summary>
    /// Manager handling game initialization, preparing services and supporting service location.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private LoadingData _LoadingData;
        
        [SerializeField]
        private Transform _LoadedObjectsRoot;
        
        public Signal ApplicationStartedSignal { get; } = new();
        public Signal LoadCompletedSignal { get; } = new();
        
        private readonly Dictionary<Type, object> _services = new();

        private static GameManager Instance;
        
        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
            Instance = this;
            ApplicationStartedSignal.Dispatch();

            InitializePlayerPrefs();
            StartCoroutine(LoadCoroutine());
        }

        /// <summary>
        /// Gets service of type T if present.
        /// </summary>
        public static T GetService<T>() where T : class, IService
        {
            if (Instance._services.TryGetValue(typeof(T), out var service))
                return service as T;
            return null;
        }

        /// <summary>
        /// Tries getting service of type T, returns success.
        /// </summary>
        public static bool TryGetService<T>(out T service) where T : class, IService
        {
            service = GetService<T>();
            return service != null;
        }

        /// <summary>
        /// Adds service of type T if not present.
        /// </summary>
        public static void AddService<T>(T service) where T : class, IService
        {
            if (Instance._services.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T).Name} already exists!");
                return;
            }
            Instance._services.Add(typeof(T), service);
        }

        /// <summary>
        /// Removes service of type T if present.
        /// </summary>
        public static void RemoveService<T>() where T : class, IService
        {
            if (!Instance._services.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T).Name} does not exist!");
                return;
            }
            Instance._services.Remove(typeof(T));
        }

        /// <summary>
        /// Instantiates prefabs specified in loading data. Should be used for spawning service monobehaviours that should be present from the start of the game. 
        /// </summary>
        private IEnumerator LoadCoroutine()
        {
            if (_LoadingData == null)
            {
                Debug.LogError("No loading data provided!");
                yield break;
            }

            foreach (var loadedObject in _LoadingData.ObjectsToLoad)
            {
                Instantiate(loadedObject, _LoadedObjectsRoot);
            }
            
            LoadCompletedSignal.Dispatch();
        }

        /// <summary>
        /// Initializes PlayerPref variables that should be always be present.
        /// </summary>
        private void InitializePlayerPrefs()
        {
            if (!PlayerPrefs.HasKey("PlayerGUID"))
            {
                var guid = Guid.NewGuid().ToString();
                PlayerPrefs.SetString("PlayerGUID", guid);
            }

            if (!PlayerPrefs.HasKey(ItemTypes.PlayerColor.ToString())) PlayerPrefs.SetString(ItemTypes.PlayerColor.ToString(), "white");
            if (!PlayerPrefs.HasKey(ItemTypes.Hat.ToString())) PlayerPrefs.SetString(ItemTypes.Hat.ToString(), "NoHat");
            if (!PlayerPrefs.HasKey(ItemTypes.PlayerTexture.ToString())) PlayerPrefs.SetString(ItemTypes.PlayerTexture.ToString(), "NoTexture");
            if (!PlayerPrefs.HasKey(ItemTypes.ParticleEffect.ToString())) PlayerPrefs.SetString(ItemTypes.ParticleEffect.ToString(), "NoParticle");
            
            PlayerPrefs.Save();
        }
    }
}

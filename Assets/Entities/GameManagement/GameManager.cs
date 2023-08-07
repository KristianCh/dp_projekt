using System.Collections;
using System.Collections.Generic;
using Entities.Events;
using UnityEngine;

namespace Entities.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private LoadingData _LoadingData;
        
        [SerializeField]
        private Transform _LoadedObjectsRoot;
        
        public Signal ApplicationStartedSignal { get; } = new();
        public Signal LoadCompletedSignal { get; } = new();
        
        private readonly Dictionary<System.Type, object> _services = new Dictionary<System.Type, object>();
        private static GameManager Instance;

        public void Start()
        {
            Instance = this;
            ApplicationStartedSignal.Dispatch();
        }

        public static T GetService<T>() where T : class, IService
        {
            if (Instance._services.TryGetValue(typeof(T), out var service))
                return service as T;
            return null;
        }

        public static bool TryGetService<T>(out T service) where T : class, IService
        {
            service = GetService<T>();
            return service != null;
        }

        public static void AddService<T>(T service) where T : class, IService
        {
            if (Instance._services.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T).Name} already exists!");
                return;
            }
            Instance._services.Add(typeof(T), service);
        }

        public static void RemoveService<T>() where T : class, IService
        {
            if (!Instance._services.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T).Name} does not exist!");
                return;
            }
            Instance._services.Remove(typeof(T));
        }

        private IEnumerator StartLoadCoroutine()
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
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Entities.GameManagement
{
    /// <summary>
    /// ScriptableObject containing prefabs that should be spawned on game launch. They are spawned in order, so objects with dependencies to services should be below the services in the list.
    /// </summary>
    [CreateAssetMenu(fileName = "LoadingData", menuName = "LoadingData", order = 0)]
    public class LoadingData : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> _ObjectsToLoad = new();
        
        public List<GameObject> ObjectsToLoad => _ObjectsToLoad;
    }
}
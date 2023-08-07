// // Author: Kristián Chovančák
// // Created: 21.07.2023
// // Copyright (c) Noxgames
// // http://www.noxgames.com/

using System.Collections.Generic;
using UnityEngine;

namespace Entities.GameManagement
{
    [CreateAssetMenu(fileName = "LoadingData", menuName = "LoadingData", order = 0)]
    public class LoadingData : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> _ObjectsToLoad = new();
        
        public List<GameObject> ObjectsToLoad => _ObjectsToLoad;
    }
}
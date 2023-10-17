using Entities.Utils;
using UnityEngine;

namespace Entities.Gameplay
{
    public class WorldObjectMovementController : MonoBehaviour
    {
        [SerializeField] 
        private float _MinZ = -20;
        public void Update()
        {
            if (transform.position.z < _MinZ)
            {
                transform.position = transform.position.WithZ(transform.position.z - Time.deltaTime);
            }
        }
    }
}
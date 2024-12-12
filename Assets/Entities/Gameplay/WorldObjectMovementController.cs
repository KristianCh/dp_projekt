using DG.Tweening;
using Entities.GameManagement;
using Entities.Utils;
using UnityEngine;

namespace Entities.Gameplay
{
    /// <summary>
    /// Controls moving object movement.
    /// </summary>
    public class WorldObjectMovementController : MonoBehaviour
    {
        [SerializeField] 
        private float _MinZ = -8;

        [SerializeField]
        private float _Speed = 4;

        protected LevelManager _levelManager;

        /// <summary>
        /// Sets initial position.
        /// </summary>
        public void Start()
        {
            _levelManager = GameManager.GetService<LevelManager>();
            transform.rotation = Quaternion.Euler(90, 0, 0);
            transform.DOLocalRotate(Vector3.zero, 1);
        }
        
        /// <summary>
        /// Updates position and destroys object if it's too far. 
        /// </summary>
        public virtual void Update()
        {
            transform.position = transform.position.WithZ(transform.position.z - Time.deltaTime * _Speed * _levelManager.SpeedMultiplier);
            if (transform.position.z < _MinZ)
            {
                Destroy(gameObject);
            }
        }
    }
}
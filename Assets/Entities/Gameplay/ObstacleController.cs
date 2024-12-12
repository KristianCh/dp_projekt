using Entities.Utils;
using UnityEngine;

namespace Entities.Gameplay
{
    /// <summary>
    /// Controls obstacle in game.
    /// </summary>
    public class ObstacleController : WorldObjectMovementController
    {
        [SerializeField]
        private ColliderEvents _ColliderEvents;

        public void Awake()
        {
            _ColliderEvents.OnTriggerEnterEvent.AddOnce(OnPickup);
        }

        /// <summary>
        /// Calls death event in level manager.
        /// </summary>
        private void OnPickup(Collider _)
        {
            _levelManager.OnDeath();
        }
    }
}
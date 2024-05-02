using Entities.Utils;
using UnityEngine;

namespace Entities.Gameplay
{
    public class ObstacleController : WorldObjectMovementController
    {
        [SerializeField]
        private ColliderEvents _ColliderEvents;

        public void Awake()
        {
            _ColliderEvents.OnTriggerEnterEvent.AddOnce(OnPickup);
        }

        private void OnPickup(Collider _)
        {
            _levelManager.OnDeath();
        }
    }
}
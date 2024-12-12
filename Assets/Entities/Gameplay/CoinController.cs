using System.Collections;
using Entities.Utils;
using UnityEngine;

namespace Entities.Gameplay
{
    /// <summary>
    /// Controls coin object in game.
    /// </summary>
    public class CoinController : WorldObjectMovementController
    {
        [SerializeField]
        private ColliderEvents _ColliderEvents;
        
        [SerializeField]
        private ParticleSystem _PickupParticleSystem;

        public void Awake()
        {
            _ColliderEvents.OnTriggerEnterEvent.AddOnce(OnPickup);
        }
        
        /// <summary>
        /// Performs rotate animation.
        /// </summary>
        public override void Update()
        {
            base.Update();
            transform.Rotate(Vector3.up, Time.deltaTime * 90f);
        }

        /// <summary>
        /// Calls level manager to update coin count and destroys itself.
        /// </summary>
        private void OnPickup(Collider _)
        {
            _levelManager.OnCoinsPickedUp();
            _PickupParticleSystem.Play();
            StartCoroutine(DestroyRoutine());
        }

        /// <summary>
        /// Waits for particles to finish playing before destroying.
        /// </summary>
        private IEnumerator DestroyRoutine()
        {
            yield return new WaitWhile(()=>_PickupParticleSystem.isPlaying);
            Destroy(gameObject);
        }
    }
}
using System.Collections;
using Entities.Utils;
using UnityEngine;

namespace Entities.Gameplay
{
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
        
        public override void Update()
        {
            base.Update();
            transform.Rotate(Vector3.up, Time.deltaTime * 90f);
        }

        private void OnPickup(Collider _)
        {
            _levelManager.OnCoinsPickedUp();
            _PickupParticleSystem.Play();
            StartCoroutine(DestroyRoutine());
        }

        private IEnumerator DestroyRoutine()
        {
            yield return new WaitWhile(()=>_PickupParticleSystem.isPlaying);
            Destroy(gameObject);
        }
    }
}
using Entities.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Utils
{
    public class ColliderEvents : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Collider _Collider;
        
        public Collider Collider => _Collider;

        private Signal<Collision> _onCollisionEnterEvent = new();
        private Signal<Collision> _onCollisionExitEvent = new();
        private Signal<Collision> _onCollisionStayEvent = new();

        private Signal<Collider> _onTriggerEnterEvent = new();
        private Signal<Collider> _onTriggerExitEvent = new();
        private Signal<Collider> _onTriggerStayEvent = new();

        public Signal<Collision> OnCollisionEnterEvent => _onCollisionEnterEvent;
        public Signal<Collision> OnCollisionExitEvent => _onCollisionExitEvent;
        public Signal<Collision> OnCollisionStayEvent => _onCollisionStayEvent;

        public Signal<Collider> OnTriggerEnterEvent => _onTriggerEnterEvent;
        public Signal<Collider> OnTriggerExitEvent => _onTriggerExitEvent;
        public Signal<Collider> OnTriggerStayEvent => _onTriggerStayEvent;
        
        public void OnCollisionEnter(Collision col) => _onCollisionEnterEvent.Dispatch(col);
        public void OnCollisionExit(Collision col) => _onCollisionExitEvent.Dispatch(col);
        public void OnCollisionStay(Collision col) => _onCollisionStayEvent.Dispatch(col);
        public void OnTriggerEnter(Collider col) => _onTriggerEnterEvent.Dispatch(col);
        public void OnTriggerExit(Collider col) => _onTriggerExitEvent.Dispatch(col);
        public void OnTriggerStay(Collider col) => _onTriggerStayEvent.Dispatch(col);
    }
}
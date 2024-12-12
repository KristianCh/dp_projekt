using System;
using System.Collections;
using DG.Tweening;
using Entities.Events;
using Entities.GameManagement;
using Entities.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Entities.Gameplay
{
    /// <summary>
    /// Controller handling player inputs.
    /// </summary>
    public class PlayerMovementController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum Quadrant
        {
            North,         // up
            East,          // right
            South,         // down
            West,          // left
            Undefined
        }

        public enum Lane
        {
            Left = 0,
            Middle = 1,
            Right = 2
        }
        
        [SerializeField]
        private Transform _playerTransform;
        [SerializeField]
        private Transform _LeftLaneTransform;
        [SerializeField]
        private Transform _MiddleLaneTransform;
        [SerializeField]
        private Transform _RightLaneTransform;
        [SerializeField]
        private Camera _Camera;
        [SerializeField]
        private ColliderEvents _ColliderEvents;
        [SerializeField]
        private MeshRenderer _MeshRenderer;

        [InlineEditor]
        [SerializeField] 
        private PlayerMovementSettings _MovementSettings;

        private Lane _lane = Lane.Middle;
        private Transform _currentLaneTransform;
        
        private Vector2 _downPosition;
        private Vector2 _upPosition;
        
        private DateTime _downTime;
        private DateTime _upTime;

        private Coroutine _jumpRoutine;
        private Coroutine _slideRoutine;
        private Coroutine _moveRoutine;
        
        public Signal<Lane> LaneChangedSignal = new();
        private float _baseY;
        private LevelManager _levelManager;
        private LevelManager LevelManager => _levelManager ??= GameManager.GetService<LevelManager>();

        
        /// <summary>
        /// Updates camera and resetting rotation.
        /// </summary>
        private void Update()
        {
            var zRot = _playerTransform.rotation.eulerAngles.z < 180 ? _playerTransform.rotation.eulerAngles.z : _playerTransform.rotation.eulerAngles.z - 360f;
            var dXRot = Time.deltaTime * 4 * LevelManager.SpeedMultiplier * (4 * Mathf.PI);
            var dZRot = Time.deltaTime * Mathf.Sign(-zRot) * 5;
            _MeshRenderer.transform.rotation *= Quaternion.Euler(dXRot, 0, 0);
            _playerTransform.rotation *= Quaternion.Euler(0, 0, dZRot);
            _Camera.transform.localPosition = _Camera.transform.localPosition.WithX(_playerTransform.localPosition.x / 2f);
        }

        /// <summary>
        /// Register on pointer down event.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            _downPosition = eventData.position;
            _downTime = DateTime.Now;
        }
        
        /// <summary>
        /// Register pointer up event. Checks swipe time and distance to validate swipe. If swipe is valid, gets quadrant and handles movement.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            _upPosition = eventData.position;
            _upTime = DateTime.Now;
            
            var swipeTime = _upTime - _downTime;
            if (swipeTime < _MovementSettings.MinSwipeTime || swipeTime > _MovementSettings.MaxSwipeTime) 
                return;
            var swipeVector = _upPosition - _downPosition;
            if (swipeVector.magnitude < _MovementSettings.MinSwipeDistance) 
                return;

            var quadrant = GetQuadrant(swipeVector);
            HandleMovement(quadrant);
        }

        
        /// <summary>
        /// Gets quadrant (direction) of the swipe.
        /// </summary>
        private Quadrant GetQuadrant(Vector2 swipeVector)
        {
            if (swipeVector.sqrMagnitude == 0) return Quadrant.Undefined;
            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
                return swipeVector.x < 0 ? Quadrant.West : Quadrant.East;
            
            return swipeVector.y > 0 ? Quadrant.North : Quadrant.South;
        }

        /// <summary>
        /// Handles switching lanes, jumping or falling based on quadrant.
        /// </summary>
        private void HandleMovement(Quadrant quadrant)
        {
            switch (quadrant)
            {
                case Quadrant.East:
                    GoRight();
                    break;
                case Quadrant.West:
                    GoLeft();
                    break;
                case Quadrant.North:
                    Jump();
                    break;
                case Quadrant.South:
                    Slide();
                    break;
            }

            void GoLeft()
            {
                if (_moveRoutine != null) return;
                switch (_lane)
                {
                    case Lane.Left:
                        return;
                    case Lane.Middle:
                        _lane = Lane.Left;
                        _currentLaneTransform = _LeftLaneTransform;
                        break;
                    case Lane.Right:
                        _lane = Lane.Middle;
                        _currentLaneTransform = _MiddleLaneTransform;
                        break;
                }
                LaneChangedSignal.Dispatch(_lane);
                _moveRoutine = StartCoroutine(MoveRoutine());
            }

            void GoRight()
            {
                if (_moveRoutine != null) return;
                switch (_lane)
                {
                    case Lane.Right:
                        return;
                    case Lane.Middle:
                        _lane = Lane.Right;
                        _currentLaneTransform = _RightLaneTransform;
                        break;
                    case Lane.Left:
                        _lane = Lane.Middle;
                        _currentLaneTransform = _MiddleLaneTransform;
                        break;
                }
                LaneChangedSignal.Dispatch(_lane);
                _moveRoutine = StartCoroutine(MoveRoutine());
            }

            void Jump()
            {
                if (_jumpRoutine != null) return;
                _jumpRoutine = StartCoroutine(JumpRoutine());
            }

            void Slide()
            {
                if (_jumpRoutine == null) return;
                _playerTransform.position = _playerTransform.position.WithY(_baseY);
                StopCoroutine(_jumpRoutine);
                _jumpRoutine = null;
            }
        }

        /// <summary>
        /// Handles player movement between lanes.
        /// </summary>
        private IEnumerator MoveRoutine()
        {
            _playerTransform.DOMoveX(_currentLaneTransform.position.x, _MovementSettings.MoveDuration).SetEase(_MovementSettings.MoveEase);
            _playerTransform.DOShakeRotation(_MovementSettings.MoveDuration, Vector3.forward * 20, 1);

            _moveRoutine = null;
            yield break;
        }

        /// <summary>
        /// Handles jumping animation.
        /// </summary>
        private IEnumerator JumpRoutine()
        {
            var elapsedTime = 0f;
            _baseY = _playerTransform.position.y;
            while (elapsedTime < _MovementSettings.JumpDuration)
            { 
                var progress = elapsedTime / _MovementSettings.JumpDuration;
                var tweenValue = 0f;
                if (progress < 0.5f) tweenValue = DOVirtual.EasedValue(0, 1, progress * 2f, Ease.OutQuad);
                else tweenValue = DOVirtual.EasedValue(1, 0, (progress - 0.5f) * 2f, Ease.InQuad);
                if (float.IsNaN(tweenValue)) Debug.LogError("Progress is NaN");
                _playerTransform.position = _playerTransform.position.WithY(_baseY + tweenValue * _MovementSettings.JumpHeight);
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }

            _playerTransform.position = _playerTransform.position.WithY(_baseY);
            _jumpRoutine = null;
        }

        private IEnumerator SlideRoutine()
        {
            yield return new WaitForSeconds(_MovementSettings.SlideDuration);
            _slideRoutine = null;
        }
    }
}
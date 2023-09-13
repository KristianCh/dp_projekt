// Author: Kristián Chovančák
// Created: 07.08.2023
// Copyright (c) Noxgames
// http://www.noxgames.com/

using System;
using System.Collections;
using DG.Tweening;
using Entities.Events;
using Entities.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Entities.Gameplay
{
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
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _downPosition = eventData.position;
            _downTime = DateTime.Now;
        }

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

        private Quadrant GetQuadrant(Vector2 swipeVector)
        {
            if (swipeVector.sqrMagnitude == 0) return Quadrant.Undefined;
            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
                return swipeVector.x < 0 ? Quadrant.West : Quadrant.East;
            
            return swipeVector.y > 0 ? Quadrant.North : Quadrant.South;
        }

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
                _slideRoutine = StartCoroutine(SlideRoutine());
            }
        }

        private IEnumerator MoveRoutine()
        {
            //yield return new WaitForSeconds(_MovementSettings.MoveDuration);
            _playerTransform.DOMoveX(_currentLaneTransform.position.x, _MovementSettings.MoveDuration).SetEase(Ease.InOutBack);

            _moveRoutine = null;
            yield break;
        }

        private IEnumerator JumpRoutine()
        {
            //yield return new WaitForSeconds(_MovementSettings.JumpDuration);
            var elapsedTime = 0f;
            var baseY = _playerTransform.position.y;
            while (elapsedTime < _MovementSettings.JumpDuration)
            { 
                var progress = elapsedTime / _MovementSettings.JumpDuration;
                var tweenValue = 0f;
                if (progress < 0.5f) tweenValue = DOVirtual.EasedValue(0, 1, progress * 2f, Ease.OutCirc);
                else tweenValue = DOVirtual.EasedValue(1, 0, (progress - 0.5f) * 2f, Ease.InCirc);
                if (float.IsNaN(tweenValue)) Debug.LogError("Progress is NaN");
                _playerTransform.position = _playerTransform.position.WithY(baseY + tweenValue * _MovementSettings.JumpHeight);
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }

            _playerTransform.position = _playerTransform.position.WithY(baseY);
            _jumpRoutine = null;
        }

        private IEnumerator SlideRoutine()
        {
            yield return new WaitForSeconds(_MovementSettings.SlideDuration);
            _slideRoutine = null;
        }
    }
}
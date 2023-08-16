// Author: Kristián Chovančák
// Created: 07.08.2023
// Copyright (c) Noxgames
// http://www.noxgames.com/

using System;
using System.Collections;
using DG.Tweening;
using Entities.Events;
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
            _playerTransform.DOMove(_currentLaneTransform.position, _MovementSettings.MoveDuration).SetEase(Ease.InOutBack);

            _moveRoutine = null;
            yield break;
        }

        private IEnumerator JumpRoutine()
        {
            yield return new WaitForSeconds(_MovementSettings.JumpDuration);
            _playerTransform.DOJump(_playerTransform.position, 100, 1, _MovementSettings.JumpDuration);
            _jumpRoutine = null;
        }

        private IEnumerator SlideRoutine()
        {
            yield return new WaitForSeconds(_MovementSettings.SlideDuration);
            _slideRoutine = null;
        }
    }
}
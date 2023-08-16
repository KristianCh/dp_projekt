// Author: Kristián Chovančák
// Created: 07.08.2023
// Copyright (c) Noxgames
// http://www.noxgames.com/

using System;
using UnityEngine;

namespace Entities.Gameplay
{
    [CreateAssetMenu(fileName = "PlayerMovementSettings", menuName = "PlayerMovementSettings", order = 0)]
    public class PlayerMovementSettings : ScriptableObject
    {
        [SerializeField]
        private float _JumpDuration = 1f;
        
        [SerializeField]
        private float _SlideDuration = 1f;
        
        [SerializeField]
        private float _MoveDuration = 1f;
        
        [SerializeField]
        private float _MinSwipeDistance = 0.5f;

        [SerializeField]
        private float _MinSwipeTimeSeconds = 0.1f;

        [SerializeField]
        private float _MaxSwipeTimeSeconds = 5f;
        
        public float JumpDuration => _JumpDuration;
        public float SlideDuration => _SlideDuration;
        public float MoveDuration => _MoveDuration;
        public float MinSwipeDistance => _MinSwipeDistance;
        public TimeSpan MinSwipeTime => TimeSpan.FromSeconds(_MinSwipeTimeSeconds);
        public TimeSpan MaxSwipeTime => TimeSpan.FromSeconds(_MaxSwipeTimeSeconds);
    }
}
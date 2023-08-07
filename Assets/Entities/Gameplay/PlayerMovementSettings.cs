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

        [SerializeReference]
        private TimeSpan _MinSwipeTime = new(0, 0, 0, 0, 10);

        [SerializeReference]
        private TimeSpan _MaxSwipeTime = new(0, 0, 0, 5, 0);
        
        public float JumpDuration => _JumpDuration;
        public float SlideDuration => _SlideDuration;
        public float MoveDuration => _MoveDuration;
        public float MinSwipeDistance => _MinSwipeDistance;
        public TimeSpan MinSwipeTime => _MinSwipeTime;
        public TimeSpan MaxSwipeTime => _MaxSwipeTime;
    }
}
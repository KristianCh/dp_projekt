using System;
using DG.Tweening;
using UnityEngine;

namespace Entities.Gameplay
{
    /// <summary>
    /// ScriptableObject defining player movement speeds and swiping parameters.
    /// </summary>
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
        
        [SerializeField]
        private float _JumpHeight = 10f;

        [SerializeField]
        private Ease _MoveEase = Ease.InOutCubic;
        
        
        
        public float JumpDuration => _JumpDuration;
        public float SlideDuration => _SlideDuration;
        public float MoveDuration => _MoveDuration;
        public float MinSwipeDistance => _MinSwipeDistance;
        public float JumpHeight => _JumpHeight;
        
        public TimeSpan MinSwipeTime => TimeSpan.FromSeconds(_MinSwipeTimeSeconds);
        public TimeSpan MaxSwipeTime => TimeSpan.FromSeconds(_MaxSwipeTimeSeconds);

        public Ease MoveEase => _MoveEase;
    }
}
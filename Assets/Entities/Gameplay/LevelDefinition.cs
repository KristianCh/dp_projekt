﻿using System;
using Entities.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Gameplay
{
    /// <summary>
    /// ScriptableObject containing settings for level values.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelDefinition", menuName = "LevelDefinition", order = 0)]
    public class LevelDefinition : ScriptableObject
    {
        [SerializeField]
        [ValidateInput("ValidateSpeed", "Start speed must be less or equal to max")]
        private float _StartSpeed = 1f;
        
        [SerializeField]
        [ValidateInput("ValidateSpeed", "Start speed must be less or equal to max")]
        private float _MaxSpeed = 3f;
        
        [SerializeField]
        private SerializableTimeSpan _SpeedupStartTime = new SerializableTimeSpan(0, 0, 30);
        
        [SerializeField]
        private SerializableTimeSpan _SpeedupEndTime = new SerializableTimeSpan(0, 5, 0);
        
        public float StartSpeed => _StartSpeed;
        public float MaxSpeed => _MaxSpeed;
        public SerializableTimeSpan SpeedupStartTime => _SpeedupStartTime;
        public SerializableTimeSpan SpeedupEndTime => _SpeedupEndTime;
        
#if UNITY_EDITOR
        private bool ValidateSpeed() => _StartSpeed <= _MaxSpeed;
        private bool ValidateSpeedupTimes() => (TimeSpan)_SpeedupStartTime <= _SpeedupEndTime;
#endif
    }
}
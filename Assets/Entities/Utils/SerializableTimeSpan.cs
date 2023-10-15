using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Utils
{
    [Serializable]
    public class SerializableTimeSpan : ISerializationCallbackReceiver, IComparable<SerializableTimeSpan>
    {
        [ShowInInspector]
        public TimeSpan timeSpan;

        [FormerlySerializedAs("_timeSpan")]
        [HideInInspector]
        [SerializeField]
        private string _TimeSpan;

        private string TimeSpanBackwardCompatibility
        {
            set
            {
                if (value != null)
                    _TimeSpan = value;
            }
        }

        public static implicit operator TimeSpan(SerializableTimeSpan uts) => uts?.timeSpan ?? TimeSpan.Zero;
        public static implicit operator SerializableTimeSpan(TimeSpan ts) => new(ts);

        public void OnAfterDeserialize()
        {
            TimeSpan.TryParse(_TimeSpan, CultureInfo.InvariantCulture, out timeSpan);
        }

        public void OnBeforeSerialize()
        {
            _TimeSpan = timeSpan.ToString();
        }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            OnBeforeSerialize();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            OnAfterDeserialize();
        }
        
        public int CompareTo(SerializableTimeSpan other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return timeSpan.CompareTo(other.timeSpan);
        }
        
        public SerializableTimeSpan()
        {
            this.timeSpan = TimeSpan.Zero;
        }
        
        public SerializableTimeSpan(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        public SerializableTimeSpan(long ticks) : 
            this(new TimeSpan(ticks)) {}
        
        public SerializableTimeSpan(int hours, int minutes, int seconds) : 
            this(new TimeSpan(hours, minutes, seconds)) {}
        
        public SerializableTimeSpan(int days, int hours, int minutes, int seconds) : 
            this(new TimeSpan(days, hours, minutes, seconds)) {}

        public SerializableTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds) : 
            this(new TimeSpan(days, hours, minutes, seconds, milliseconds)) {}
    }

    [Conditional("UNITY_EDITOR")]
    public class TimeSpanDrawerSettingsAttribute : Attribute
    {
        public MillisecondsDrawType MillisecondsDrawType;

        public TimeSpanDrawerSettingsAttribute() 
        {
        }

        public TimeSpanDrawerSettingsAttribute(MillisecondsDrawType millisecondsDrawType)
        {
            MillisecondsDrawType = millisecondsDrawType;
        }
    }

    public enum MillisecondsDrawType
    {
        None,
        SecondsDecimal,
        SeparateField
    }

    
#if UNITY_EDITOR
    public static class TimeSpanDrawerUtils
    {
        public static void DrawTimeSpanFieldMsSeparate(GUIContent label, TimeSpan currentValue, Action<TimeSpan> valueSetter)
        {
            SirenixEditorGUI.BeginVerticalPropertyLayout(label);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            
            var days = SirenixEditorFields.IntField(currentValue.Days);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "D", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(".", GUILayout.Width(5));
            var hours = SirenixEditorFields.IntField(currentValue.Hours);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "h", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            var minutes = SirenixEditorFields.IntField(currentValue.Minutes);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "m", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(":", GUILayout.Width(5));
            var seconds = SirenixEditorFields.IntField(currentValue.Seconds);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "s", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(".", GUILayout.Width(5));
            var milliSeconds = SirenixEditorFields.IntField(currentValue.Milliseconds);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "ms", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(1);
            if (EditorGUI.EndChangeCheck())
            {
                valueSetter?.Invoke(new TimeSpan(days, hours, minutes, seconds, milliSeconds));
            }
            SirenixEditorGUI.EndVerticalPropertyLayout();
        }
        
        public static void DrawTimeSpanFieldMsDecimal(GUIContent label, TimeSpan currentValue, Action<TimeSpan> valueSetter)
        {
            SirenixEditorGUI.BeginHorizontalPropertyLayout(label);
            EditorGUI.BeginChangeCheck();

            var days = SirenixEditorFields.IntField(currentValue.Days);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "D", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(".", GUILayout.Width(5));
            var hours = SirenixEditorFields.IntField(currentValue.Hours);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "h", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(":", GUILayout.Width(5));
            var minutes = SirenixEditorFields.IntField(currentValue.Minutes);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "m", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(":", GUILayout.Width(5));
            var seconds = SirenixEditorFields.FloatField(currentValue.Seconds + currentValue.Milliseconds / 1000f);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "s", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            
            var secondsTimeSpan = TimeSpan.FromSeconds(seconds);
            
            if (EditorGUI.EndChangeCheck())
            {
                valueSetter?.Invoke(new TimeSpan(days, hours, minutes, Mathf.FloorToInt((float)secondsTimeSpan.TotalSeconds), secondsTimeSpan.Milliseconds));
            }
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
        
        public static void DrawTimeSpanField(GUIContent label, TimeSpan currentValue, Action<TimeSpan> valueSetter)
        {
            SirenixEditorGUI.BeginHorizontalPropertyLayout(label);
            EditorGUI.BeginChangeCheck();

            var days = SirenixEditorFields.IntField(currentValue.Days);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "D", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(".", GUILayout.Width(5));
            var hours = SirenixEditorFields.IntField(currentValue.Hours);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "h", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(":", GUILayout.Width(5));
            var minutes = SirenixEditorFields.IntField(currentValue.Minutes);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "m", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            EditorGUILayout.LabelField(":", GUILayout.Width(5));
            var seconds = SirenixEditorFields.IntField(currentValue.Seconds);
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), "s", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            
            if (EditorGUI.EndChangeCheck())
            {
                valueSetter?.Invoke(new TimeSpan(days, hours, minutes, seconds));
            }
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }

    [UsedImplicitly]
    public sealed class UTimeSpanDrawer : OdinValueDrawer<SerializableTimeSpan>
    {
        private TimeSpanDrawerSettingsAttribute _settingsAttribute;
        
        protected override void Initialize()
        {
            base.Initialize();
            _settingsAttribute = Property.GetAttribute<TimeSpanDrawerSettingsAttribute>() ?? new TimeSpanDrawerSettingsAttribute();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            switch (_settingsAttribute.MillisecondsDrawType)
            {
                case MillisecondsDrawType.SecondsDecimal: 
                    TimeSpanDrawerUtils.DrawTimeSpanFieldMsDecimal(label, ValueEntry.SmartValue.timeSpan, ValueSetter);
                    break;
                
                case MillisecondsDrawType.SeparateField: 
                    TimeSpanDrawerUtils.DrawTimeSpanFieldMsSeparate(label, ValueEntry.SmartValue.timeSpan, ValueSetter);
                    break;

                case MillisecondsDrawType.None:
                default: 
                    TimeSpanDrawerUtils.DrawTimeSpanField(label, ValueEntry.SmartValue.timeSpan, ValueSetter);
                    break;
            }
        }

        private void ValueSetter(TimeSpan value)
        {
            ValueEntry.SmartValue.timeSpan = value;
            ValueEntry.ApplyChanges();
            Property.MarkSerializationRootDirty();
        }
    }

    [UsedImplicitly]
    public sealed class TimeSpanDrawer : OdinValueDrawer<TimeSpan>
    {
        private TimeSpanDrawerSettingsAttribute _settingsAttribute;
        
        protected override void Initialize()
        {
            base.Initialize();
            _settingsAttribute = Property.GetAttribute<TimeSpanDrawerSettingsAttribute>() ?? new TimeSpanDrawerSettingsAttribute();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            switch (_settingsAttribute.MillisecondsDrawType)
            {
                case MillisecondsDrawType.SecondsDecimal: 
                    TimeSpanDrawerUtils.DrawTimeSpanFieldMsDecimal(label, ValueEntry.SmartValue, ValueSetter); 
                    break;
                
                case MillisecondsDrawType.SeparateField: 
                    TimeSpanDrawerUtils.DrawTimeSpanFieldMsSeparate(label, ValueEntry.SmartValue, ValueSetter); 
                    break;

                case MillisecondsDrawType.None:
                default: 
                    TimeSpanDrawerUtils.DrawTimeSpanField(label, ValueEntry.SmartValue, ValueSetter); 
                    break;
            }
        }

        private void ValueSetter(TimeSpan value)
        {
            ValueEntry.SmartValue = value;
            ValueEntry.ApplyChanges();
            Property.MarkSerializationRootDirty();
        }
    }
#endif
}


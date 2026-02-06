using System;
using Game.Runtime.WeatherSystem;
using Game.Runtime.WeatherSystem.WeatherTween;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomPropertyDrawer(typeof(WeatherProperty))]
    public class WeatherPropertyDrawer : PropertyDrawer
    {
        private static float LineHeight => EditorGUIUtility.singleLineHeight;
        
        private SerializedProperty PropertyType;
        private SerializedProperty Entity;
        private SerializedProperty Property;
        private SerializedProperty FloatValue;
        private SerializedProperty VectorValue;
        private SerializedProperty ColorValue;

        private int _currentPropertyIndex = 0;
        private WeatherEntityType _currentEntityType;
        private WeatherParameterType _currentParameterType;
        private WeatherParameterType[] _currentParametersTypeList;
        private string[] _currentParametersNameList;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PropertyType = property.FindPropertyRelative("Type");
            Entity = PropertyType.FindPropertyRelative("Entity");
            Property = PropertyType.FindPropertyRelative("Parameter");
            FloatValue = property.FindPropertyRelative("FloatValue");
            VectorValue = property.FindPropertyRelative("VectorValue");
            ColorValue = property.FindPropertyRelative("ColorValue");
            
            EditorGUI.BeginProperty(position, label, property);
            
            var foldoutRect = new Rect(position)
            {
                height = LineHeight
            };
            
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, $"{_currentEntityType.ToString()} - {_currentParameterType.ToString()}");
            EditorGUI.EndFoldoutHeaderGroup();
            
            if(property.isExpanded)
            {
                EditorGUILayout.PropertyField(Entity);

                var entityType = (WeatherEntityType)Entity.enumValueIndex;
                if (_currentParametersTypeList == null 
                    || _currentParametersNameList == null 
                    || _currentParametersTypeList.Length == 0 
                    || _currentParametersNameList.Length == 0 
                    || entityType != _currentEntityType)
                {
                    _currentEntityType = entityType;
                    _currentParametersTypeList = _currentEntityType.GetProperties();

                    _currentParametersNameList = new string[_currentParametersTypeList.Length];
                    for (int i = 0; i < _currentParametersTypeList.Length; i++)
                    {
                        _currentParametersNameList[i] = _currentParametersTypeList[i].ToString();
                    }
                }

                if (_currentPropertyIndex > _currentParametersNameList.Length - 1)
                    _currentPropertyIndex = 0;
                
                _currentPropertyIndex =
                    EditorGUILayout.Popup(Property.displayName, _currentPropertyIndex, _currentParametersNameList);
                
                Property.enumValueIndex = (int)_currentParametersTypeList[_currentPropertyIndex];

                _currentParameterType = _currentParametersTypeList[_currentPropertyIndex];
                
                var propertyType = _currentParametersTypeList[_currentPropertyIndex].GetPropertyType();
                if (propertyType == typeof(float))
                    EditorGUILayout.PropertyField(FloatValue);
                else if (propertyType == typeof(Vector2))
                    EditorGUILayout.PropertyField(VectorValue);
                else if (propertyType == typeof(Color))
                    EditorGUILayout.PropertyField(ColorValue);
            }
            EditorGUI.EndProperty();
        }
    }
}
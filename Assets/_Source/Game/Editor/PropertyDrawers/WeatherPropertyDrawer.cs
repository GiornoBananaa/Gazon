using System;
using Game.Runtime.Weather.WeatherTween;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomPropertyDrawer(typeof(WeatherProperty))]
    public class WeatherPropertyDrawer : PropertyDrawer
    {
        private static float LineHeight => EditorGUIUtility.singleLineHeight;
        
        private SerializedProperty Entity;
        private SerializedProperty Property;
        private SerializedProperty FloatValue;
        private SerializedProperty VectorValue;
        private SerializedProperty ColorValue;

        private int _currentPropertyIndex = 0;
        private WeatherEntityType _currentEntityType;
        private WeatherPropertyType _currentPropertyType;
        private WeatherPropertyType[] _currentPropertyTypeList;
        private string[] _currentPropertyList;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Entity = property.FindPropertyRelative("Entity");
            Property = property.FindPropertyRelative("Property");
            FloatValue = property.FindPropertyRelative("FloatValue");
            VectorValue = property.FindPropertyRelative("VectorValue");
            ColorValue = property.FindPropertyRelative("ColorValue");
            
            EditorGUI.BeginProperty(position, label, property);
            
            var foldoutRect = new Rect(position)
            {
                height = LineHeight
            };
            
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, $"{_currentEntityType.ToString()} - {_currentPropertyType.ToString()}");
            EditorGUI.EndFoldoutHeaderGroup();
            
            if(property.isExpanded)
            {
                EditorGUILayout.PropertyField(Entity);

                var entityType = (WeatherEntityType)Entity.enumValueIndex;
                if (entityType != _currentEntityType)
                {
                    _currentEntityType = entityType;
                    _currentPropertyTypeList = _currentEntityType.GetProperties();

                    _currentPropertyList = new string[_currentPropertyTypeList.Length];
                    for (int i = 0; i < _currentPropertyTypeList.Length; i++)
                    {
                        _currentPropertyList[i] = _currentPropertyTypeList[i].ToString();
                    }
                }

                _currentPropertyIndex =
                    EditorGUILayout.Popup(Property.displayName, _currentPropertyIndex, _currentPropertyList);

                Property.enumValueIndex = (int)_currentPropertyTypeList[_currentPropertyIndex];

                _currentPropertyType = _currentPropertyTypeList[_currentPropertyIndex];
                
                var propertyType = _currentPropertyTypeList[_currentPropertyIndex].GetPropertyType();
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
using System;
using Multiverse.Utils;
using UnityEditor;
using UnityEngine;

namespace Multiverse
{
    [CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
    public class ReadOnlyFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var text = property.propertyType switch
            {
                SerializedPropertyType.Integer => property.intValue.ToString(),
                SerializedPropertyType.Boolean => property.boolValue.ToString(),
                SerializedPropertyType.Float => property.floatValue.ToString("0.0000"),
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Color => property.colorValue.ToString(),
                SerializedPropertyType.ObjectReference => property.objectReferenceValue.ToString(),
                SerializedPropertyType.Enum => property.enumDisplayNames[property.enumValueIndex],
                SerializedPropertyType.Vector2 => property.vector2Value.ToString(),
                SerializedPropertyType.Vector3 => property.vector3Value.ToString(),
                SerializedPropertyType.Vector4 => property.vector4Value.ToString(),
                SerializedPropertyType.Rect => property.rectValue.ToString(),
                SerializedPropertyType.AnimationCurve => property.animationCurveValue.ToString(),
                SerializedPropertyType.Bounds => property.boundsValue.ToString(),
                SerializedPropertyType.Quaternion => property.quaternionValue.ToString(),
                SerializedPropertyType.ExposedReference => property.exposedReferenceValue.ToString(),
                SerializedPropertyType.Vector2Int => property.vector2IntValue.ToString(),
                SerializedPropertyType.Vector3Int => property.vector3IntValue.ToString(),
                SerializedPropertyType.RectInt => property.rectIntValue.ToString(),
                SerializedPropertyType.BoundsInt => property.boundsIntValue.ToString(),
                _ => throw new ArgumentOutOfRangeException()
            };
            EditorGUI.LabelField(position, label.text, text);
        }
    }
}
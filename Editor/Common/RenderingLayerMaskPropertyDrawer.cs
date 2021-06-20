using Aarthificial.PixelGraphics.Common;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.PixelGraphics.Editor.Common
{
    [CustomPropertyDrawer(typeof(RenderingLayerMask))]
    public class RenderingLayerMaskPropertyDrawer : PropertyDrawer
    {
        private readonly string[] _options = new string[32];
        private bool _optionsCreated;

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label
        )
        {
            CreateOptions();
            var maskProperty = property.FindPropertyRelative("mask");

            EditorGUI.BeginProperty(position, label, property);
            maskProperty.intValue = EditorGUI.MaskField(
                position,
                label,
                maskProperty.intValue,
                _options
            );

            EditorGUI.EndProperty();
        }

        private void CreateOptions()
        {
            if (_optionsCreated) return;

            for (var i = 0; i < _options.Length; i++)
            {
                _options[i] = $"Layer{i + 1}";
            }

            _optionsCreated = true;
        }
    }
}
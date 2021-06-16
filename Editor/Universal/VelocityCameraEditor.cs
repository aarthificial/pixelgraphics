using Aarthificial.PixelGraphics.Common;
using Aarthificial.PixelGraphics.Universal;
using UnityEditor;

namespace Aarthificial.PixelGraphics.Editor.Universal
{
    [CustomEditor(typeof(VelocityCamera))]
    public class VelocityCameraEditor : UnityEditor.Editor
    {
        private SerializedProperty _passSettings;
        private SerializedProperty _ppu;
        private SerializedProperty _layerMask;
        private SerializedProperty _textureScale;
        private SerializedProperty _simulationSettings;

        private void OnEnable()
        {
            _simulationSettings = serializedObject.FindProperty("simulation");
            _passSettings = serializedObject.FindProperty("settings");
            _ppu = _passSettings.FindPropertyRelative(nameof(VelocityPassSettings.pixelsPerUnit));
            _layerMask = _passSettings.FindPropertyRelative(nameof(VelocityPassSettings.layerMask));
            _textureScale = _passSettings.FindPropertyRelative(nameof(VelocityPassSettings.textureScale));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_ppu);
            EditorGUILayout.PropertyField(_layerMask);
            EditorGUILayout.PropertyField(_textureScale);
            EditorGUILayout.PropertyField(_simulationSettings);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
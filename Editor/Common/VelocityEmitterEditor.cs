using System.Reflection;
using Aarthificial.PixelGraphics.Common;
using Aarthificial.PixelGraphics.Forward;
using Aarthificial.PixelGraphics.Universal;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aarthificial.PixelGraphics.Editor.Common
{
    [CustomEditor(typeof(VelocityEmitter))]
    public class VelocityEmitterEditor : UnityEditor.Editor
    {
        private static readonly GUIContent RigidbodyLabel = EditorGUIUtility.TrTextContent(
            "Rigidbody",
            "The rigidbody component used as the source of velocity."
        );

        private SerializedProperty _mode;
        private SerializedProperty _rigidbody;
        private SerializedProperty _rigidbody2D;
        private SerializedProperty _maxSpeed;
        private SerializedProperty _remapping;
        private SerializedProperty _customVelocity;

        private void OnEnable()
        {
            _mode = serializedObject.FindProperty("mode");
            _rigidbody = serializedObject.FindProperty("rb");
            _rigidbody2D = serializedObject.FindProperty("rb2D");
            _maxSpeed = serializedObject.FindProperty("maxSpeed");
            _remapping = serializedObject.FindProperty("remapping");
            _customVelocity = serializedObject.FindProperty(nameof(VelocityEmitter.customVelocity));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_mode);
            switch ((VelocityEmitter.EmitterMode) _mode.intValue)
            {
                case VelocityEmitter.EmitterMode.Rigidbody:
                    EditorGUILayout.PropertyField(_rigidbody, RigidbodyLabel);
                    break;
                case VelocityEmitter.EmitterMode.Rigidbody2D:
                    EditorGUILayout.PropertyField(_rigidbody2D, RigidbodyLabel);
                    break;
                case VelocityEmitter.EmitterMode.Custom:
                    EditorGUILayout.PropertyField(_customVelocity);
                    break;
            }

            EditorGUILayout.PropertyField(_maxSpeed);
            EditorGUILayout.PropertyField(_remapping);

            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("GameObject/2D Object/Sprites/Velocity Emitter")]
        private static void CreateCustomEmitter(MenuCommand menuCommand)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/com.aarthificial.pixelgraphics/Runtime/Prefabs/Velocity Emitter.prefab"
            );
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null) return;

            PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            GameObjectUtility.SetParentAndAlign(instance, menuCommand.context as GameObject);

            if (TryGetRendererData(out var data))
            {
                foreach (var feature in data.rendererFeatures)
                {
                    if (feature is VelocityRenderFeature velocityRenderFeature)
                    {
                        instance.layer = GetFirstLayerFromMask(velocityRenderFeature.settings.layerMask);
                        break;
                    }
                }
            }

            var camera = FindObjectOfType<VelocityCamera>();
            if (camera != null)
            {
                instance.layer = GetFirstLayerFromMask(camera.settings.layerMask);
            }

            Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);
            Selection.activeObject = instance;
        }

        private static int GetFirstLayerFromMask(int mask)
        {
            var layer = 0;
            while (layer < 32)
            {
                if ((mask & (1 << layer)) != 0)
                    break;
                layer++;
            }

            return layer;
        }

        private static bool TryGetRendererData(out ForwardRendererData data)
        {
            if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset pipeline))
            {
                data = null;
                return false;
            }

            data = typeof(UniversalRenderPipelineAsset).GetProperty(
                    "scriptableRendererData",
                    BindingFlags.Instance | BindingFlags.NonPublic
                )
                ?.GetValue(pipeline) as ForwardRendererData;

            return data != null;
        }
    }
}
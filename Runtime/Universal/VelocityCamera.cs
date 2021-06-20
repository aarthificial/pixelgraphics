using Aarthificial.PixelGraphics.Common;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aarthificial.PixelGraphics.Universal
{
    [AddComponentMenu("PixelGraphics/Velocity Camera")]
    [RequireComponent(typeof(Camera))]
    public class VelocityCamera : MonoBehaviour
    {
        [SerializeField] internal VelocityPassSettings settings;
        [SerializeField] private SimulationSettings simulation;
        [SerializeField] [HideInInspector] private Shader blitShader;

        private RenderTexture _temporaryVelocityTexture;
        private RenderTexture _previousVelocityTexture;
        private RenderTexture _velocityTexture;
        private Camera _originalCamera;
        private Camera _velocityCamera;
        private Vector2Int _cameraSize;
        private Vector2Int _textureSize;
        private Vector2 _previousPosition;
        private Material _blitMaterial;

        private void Awake()
        {
#if UNITY_EDITOR
            if (blitShader == null)
                blitShader = Shader.Find(ShaderIds.VelocityBlitShader);
#endif
            _blitMaterial = CoreUtils.CreateEngineMaterial(blitShader);
            _originalCamera = GetComponent<Camera>();
            _originalCamera.cullingMask &= ~settings.layerMask;
        }

        private void OnValidate()
        {
            if (blitShader == null)
                blitShader = Shader.Find(ShaderIds.VelocityBlitShader);
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += HandleBeginCameraRendering;

            if (_velocityCamera == null)
            {
                var go = new GameObject("Velocity Camera", typeof(Camera));
                go.transform.SetParent(transform, false);
                _velocityCamera = go.GetComponent<Camera>();
            }

            _velocityCamera.CopyFrom(_originalCamera);
            UpdateTextures(true);
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= HandleBeginCameraRendering;
            RenderTexture.ReleaseTemporary(_temporaryVelocityTexture);

            if (_velocityCamera != null)
                Destroy(_velocityCamera.gameObject);
            _velocityCamera = null;
        }

        private void HandleBeginCameraRendering(ScriptableRenderContext context, Camera currentCamera)
        {
            if (!ReferenceEquals(_velocityCamera, currentCamera)) return;

            _velocityCamera.CopyFrom(_originalCamera);
            _velocityCamera.targetTexture = _temporaryVelocityTexture;
            _velocityCamera.cullingMask = settings.layerMask;
            _velocityCamera.backgroundColor = Color.clear;

            float height = 2 * _originalCamera.orthographicSize * settings.pixelsPerUnit;
            float width = height * _originalCamera.aspect;

            var cameraPosition = (Vector2) _originalCamera.transform.position;
            var delta = _previousPosition - cameraPosition;
            _previousPosition = cameraPosition;
            var screenDelta = _originalCamera.projectionMatrix * _originalCamera.worldToCameraMatrix * delta;
            var cmd = CommandBufferPool.Get();
            cmd.Clear();

            cmd.SetGlobalVector(ShaderIds.VelocitySimulationParams, simulation.Value);
            cmd.SetGlobalVector(ShaderIds.CameraPositionDelta, screenDelta * 0.5f);
            cmd.SetGlobalTexture(ShaderIds.VelocityTexture, _velocityTexture);
            cmd.SetGlobalTexture(ShaderIds.PreviousVelocityTexture, _previousVelocityTexture);
            cmd.SetGlobalTexture(ShaderIds.TemporaryVelocityTexture, _temporaryVelocityTexture);
            cmd.SetGlobalVector(
                ShaderIds.PixelScreenParams,
                new Vector4(
                    width,
                    height,
                    settings.pixelsPerUnit,
                    1 / settings.pixelsPerUnit
                )
            );

            // TODO Implement proper double buffering
            cmd.Blit(_velocityTexture, _previousVelocityTexture);

            CoreUtils.SetRenderTarget(cmd, _velocityTexture);
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.SetViewport(new Rect(0, 0, _textureSize.x, _textureSize.y));
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _blitMaterial, 0, 1);

            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
        }

        private void UpdateTextures(bool initial = false)
        {
            if (!initial)
            {
                RenderTexture.ReleaseTemporary(_temporaryVelocityTexture);
                RenderTexture.ReleaseTemporary(_previousVelocityTexture);
                RenderTexture.ReleaseTemporary(_velocityTexture);
            }

            _textureSize.x = Mathf.FloorToInt(_originalCamera.pixelWidth * settings.textureScale);
            _textureSize.y = Mathf.FloorToInt(_originalCamera.pixelHeight * settings.textureScale);

            _temporaryVelocityTexture = RenderTexture.GetTemporary(
                _textureSize.x,
                _textureSize.y,
                0,
                GraphicsFormat.R16G16B16A16_SFloat
            );
            _previousVelocityTexture = RenderTexture.GetTemporary(
                _textureSize.x,
                _textureSize.y,
                0,
                GraphicsFormat.R16G16B16A16_SFloat
            );
            _velocityTexture = RenderTexture.GetTemporary(
                _textureSize.x,
                _textureSize.y,
                0,
                GraphicsFormat.R16G16B16A16_SFloat
            );
        }

        private void Update()
        {
            if (_cameraSize.x == _originalCamera.pixelWidth && _cameraSize.y == _originalCamera.pixelHeight) return;

            _cameraSize.x = _originalCamera.pixelWidth;
            _cameraSize.y = _originalCamera.pixelHeight;

            UpdateTextures();
        }
    }
}
using System.Collections.Generic;
using Aarthificial.PixelGraphics.Common;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aarthificial.PixelGraphics.Forward
{
    public class VelocityRenderPass : ScriptableRenderPass
    {
        private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();
        private readonly ProfilingSampler _profilingSampler;
        private readonly RenderTargetHandle _temporaryVelocityTarget;
        private readonly RenderTargetHandle _velocityTarget;
        private readonly Material _emitterMaterial;
        private readonly Material _blitMaterial;

        private VelocityPassSettings _passSettings;
        private SimulationSettings _simulationSettings;
        private FilteringSettings _filteringSettings;
        private Vector2 _previousPosition;

        public VelocityRenderPass(Material emitterMaterial, Material blitMaterial)
        {
            _emitterMaterial = emitterMaterial;
            _blitMaterial = blitMaterial;

            _temporaryVelocityTarget.Init("_PG_TemporaryVelocityTextureTarget");
            _velocityTarget.Init("_VelocityTarget");

            _shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            _shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            _shaderTagIdList.Add(new ShaderTagId("Universal2D"));
            _shaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
            _shaderTagIdList.Add(new ShaderTagId("LightweightForward"));

            _filteringSettings = new FilteringSettings(RenderQueueRange.transparent);
            _profilingSampler = new ProfilingSampler(nameof(VelocityRenderPass));

            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        public void Setup(
            VelocityPassSettings passSettings,
            SimulationSettings simulationSettings
        )
        {
            _passSettings = passSettings;
            _simulationSettings = simulationSettings;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Clear();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                ref var cameraData = ref renderingData.cameraData;

                int textureWidth = Mathf.FloorToInt(cameraData.camera.pixelWidth * _passSettings.textureScale);
                int textureHeight = Mathf.FloorToInt(cameraData.camera.pixelHeight * _passSettings.textureScale);

                float height = 2 * cameraData.camera.orthographicSize * _passSettings.pixelsPerUnit;
                float width = height * cameraData.camera.aspect;

                var cameraPosition = (Vector2) cameraData.GetViewMatrix().GetColumn(3);
                var delta = cameraPosition - _previousPosition;
                var screenDelta = cameraData.GetProjectionMatrix() * cameraData.GetViewMatrix() * delta;
                _previousPosition = cameraPosition;

                cmd.GetTemporaryRT(
                    _temporaryVelocityTarget.id,
                    textureWidth,
                    textureHeight,
                    0,
                    FilterMode.Bilinear,
                    GraphicsFormat.R16G16B16A16_SFloat
                );
                cmd.GetTemporaryRT(
                    _velocityTarget.id,
                    textureWidth,
                    textureHeight,
                    0,
                    FilterMode.Bilinear,
                    GraphicsFormat.R16G16B16A16_SFloat
                );

                cmd.SetGlobalVector(ShaderIds.CameraPositionDelta, screenDelta / 2);
                cmd.SetGlobalTexture(ShaderIds.VelocityTexture, _velocityTarget.id);
                cmd.SetGlobalTexture(ShaderIds.PreviousVelocityTexture, _temporaryVelocityTarget.id);
                cmd.SetGlobalVector(ShaderIds.VelocitySimulationParams, _simulationSettings.Value);
                cmd.SetGlobalVector(
                    ShaderIds.PixelScreenParams,
                    new Vector4(
                        width,
                        height,
                        _passSettings.pixelsPerUnit,
                        1 / _passSettings.pixelsPerUnit
                    )
                );

                CoreUtils.SetRenderTarget(cmd, _velocityTarget.id);
                cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                cmd.SetViewport(new Rect(0, 0, textureWidth, textureHeight));
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _blitMaterial, 0, 0);
                cmd.SetViewProjectionMatrices(cameraData.GetViewMatrix(), cameraData.GetProjectionMatrix());
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                if (!cameraData.isPreviewCamera && !cameraData.isSceneViewCamera)
                {
                    var drawingSettings = CreateDrawingSettings(
                        _shaderTagIdList,
                        ref renderingData,
                        SortingCriteria.CommonTransparent
                    );

                    if (_passSettings.layerMask != 0)
                    {
                        _filteringSettings.layerMask = _passSettings.layerMask;
                        _filteringSettings.renderingLayerMask = uint.MaxValue;
                        drawingSettings.overrideMaterial = null;
                        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
                    }

                    if (_passSettings.renderingLayerMask != 0)
                    {
                        _filteringSettings.layerMask = -1;
                        _filteringSettings.renderingLayerMask = _passSettings.renderingLayerMask;
                        drawingSettings.overrideMaterial = _emitterMaterial;
                        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
                    }
                }

                // TODO Implement proper double buffering
                cmd.Blit(_velocityTarget.id, _temporaryVelocityTarget.id);
#if UNITY_EDITOR
                if (_passSettings.preview)
                    cmd.Blit(_velocityTarget.id, colorAttachment);
#endif
                CoreUtils.SetRenderTarget(cmd, colorAttachment);
                cmd.ReleaseTemporaryRT(_temporaryVelocityTarget.id);
                cmd.ReleaseTemporaryRT(_velocityTarget.id);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
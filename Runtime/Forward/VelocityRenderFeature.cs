using Aarthificial.PixelGraphics.Common;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aarthificial.PixelGraphics.Forward
{
    public class VelocityRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] internal VelocityPassSettings settings;
        [SerializeField] internal SimulationSettings simulation;

        [SerializeField, HideInInspector, Reload("Runtime/Shaders/Velocity/Emitter.shader")]
        private Shader emitterShader;

        [SerializeField, HideInInspector, Reload("Runtime/Shaders/Velocity/Blit.shader")]
        private Shader blitShader;

        private VelocityRenderPass _pass;
        private Material _emitterMaterial;
        private Material _blitMaterial;

        public override void Create()
        {
#if UNITY_EDITOR
            if (blitShader == null)
                blitShader = Shader.Find(ShaderIds.VelocityBlitShader);
            if (emitterShader == null)
                emitterShader = Shader.Find(ShaderIds.VelocityEmitterShader);
#endif
            _emitterMaterial = CoreUtils.CreateEngineMaterial(emitterShader);
            _blitMaterial = CoreUtils.CreateEngineMaterial(blitShader);
            _pass = new VelocityRenderPass(_emitterMaterial, _blitMaterial);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _pass.ConfigureTarget(renderer.cameraColorTarget);
            _pass.Setup(settings, simulation);
            renderer.EnqueuePass(_pass);
        }
    }
}
using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    internal static class ShaderIds
    {
        internal const string VelocityBlitShader = "Hidden/PixelGraphics/Velocity/Blit";
        internal const string VelocityEmitterShader = "PixelGraphics/Velocity/Emitter";

        internal static readonly int CameraPositionDelta = Shader.PropertyToID("_PG_CameraPositionDelta");
        internal static readonly int PositionDelta = Shader.PropertyToID("_PG_PositionDelta");
        internal static readonly int VelocityTexture = Shader.PropertyToID("_PG_VelocityTexture");
        internal static readonly int PreviousVelocityTexture = Shader.PropertyToID("_PG_PreviousVelocityTexture");
        internal static readonly int TemporaryVelocityTexture = Shader.PropertyToID("_PG_TemporaryVelocityTexture");
        internal static readonly int PixelScreenParams = Shader.PropertyToID("_PG_PixelScreenParams");
        internal static readonly int VelocitySimulationParams = Shader.PropertyToID("_PG_VelocitySimulationParams");
    }
}
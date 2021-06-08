#ifndef VELOCITYSIMULATION_INCLUDE
#define VELOCITYSIMULATION_INCLUDE
#define SAMPLE_PREVIOUS_VELOCITY(coordX, coordY) SAMPLE_TEXTURE2D(_PG_PreviousVelocityTexture, sampler_PG_PreviousVelocityTexture, uv - _PG_CameraPositionDelta.xy + float2(coordX, coordY))

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
TEXTURE2D(_PG_PreviousVelocityTexture);
SAMPLER(sampler_PG_PreviousVelocityTexture);
float4 _PG_PreviousVelocityTexture_TexelSize;
TEXTURE2D(_PG_TemporaryVelocityTexture);
SAMPLER(sampler_PG_TemporaryVelocityTexture);

float4 _PG_VelocitySimulationParams;
float4 _PG_CameraPositionDelta;
CBUFFER_END

float4 SimulateVelocity(float2 uv)
{
    float4 previousData = SAMPLE_PREVIOUS_VELOCITY(0, 0);
    float2 offset = _PG_PreviousVelocityTexture_TexelSize.xy;

    float4 neighbouringData = SAMPLE_PREVIOUS_VELOCITY(offset.x, 0);
    neighbouringData += SAMPLE_PREVIOUS_VELOCITY(-offset.x, 0);
    neighbouringData += SAMPLE_PREVIOUS_VELOCITY(0, offset.y);
    neighbouringData += SAMPLE_PREVIOUS_VELOCITY(0, -offset.y);

    neighbouringData *= 0.25f;
    previousData = lerp(previousData, neighbouringData, _PG_VelocitySimulationParams.z);

    float2 distance = previousData.xy;
    float2 velocity = previousData.zw;

    if (uv.x < offset.x
        || uv.x > 1 - offset.x
        || uv.y < offset.y
        || uv.y > 1 - offset.y
    )
    {
        velocity = 0;
        distance = 0;
    }

    float dt = min(unity_DeltaTime.x, _PG_VelocitySimulationParams.w);
    float2 acceleration = -distance * _PG_VelocitySimulationParams.x - velocity * _PG_VelocitySimulationParams.y;
    velocity += acceleration * dt;
    distance += velocity * dt;

    return float4(distance.x, distance.y, velocity.x, velocity.y);
}

#endif //VELOCITYSIMULATION_INCLUDE

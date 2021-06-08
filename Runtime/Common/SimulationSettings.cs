using System;
using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    [Serializable]
    public class SimulationSettings
    {
        [SerializeField] private float stiffness = 70;
        [SerializeField] private float damping = 3;
        [SerializeField] private float blurStrength = 0.5f;
        [SerializeField] private float minDeltaTime = 0.034f;

        public Vector4 Value =>
            new Vector4(
                stiffness,
                damping,
                blurStrength,
                minDeltaTime
            );
    }
}
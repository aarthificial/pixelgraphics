using System;
using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    [Serializable]
    public class SimulationSettings
    {
        [SerializeField]
        [Tooltip("The spring constant.")]
        private float stiffness = 70;

        [SerializeField]
        [Tooltip("The linear damping coefficient.")]
        private float damping = 3;

        [SerializeField]
        [Tooltip(
            "The strength of the blur.\nOther factors, like the velocity texture scale, can also affect blurring."
        )]
        private float blurStrength = 0.5f;

        [SerializeField]
        [Tooltip(
            "The maximum delta time in seconds.\nUsed to keep the simulation stable in case of FPS drops. The default values is 1/30 (30 FPS)"
        )]
        private float maxDeltaTime = 0.034f;

        public Vector4 Value =>
            new Vector4(
                stiffness,
                damping,
                blurStrength,
                maxDeltaTime
            );
    }
}
using System;
using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    [Serializable]
    public class VelocityPassSettings
    {
        [Tooltip(
            "The amount of pixels that make up one unit of the Scene.\nSet this value to match the PPU value of Sprites in the Scene."
        )]
        public float pixelsPerUnit = 24;

        [Tooltip(
            "Which rendering layers the velocity camera renders.\nAllows you to use sprites as velocity emitters while keeping them visible in the default camera."
        )]
        public RenderingLayerMask renderingLayerMask;

        [Tooltip(
            "Which layers the velocity camera renders.\nThe layers should be excluded from the default camera's culling mask"
        )]
        public LayerMask layerMask;

        [Range(0.01f, 1f)]
        [Tooltip(
            "The resolution of the velocity texture relative to the screen resolution. 1.0 means full screen size."
        )]
        public float textureScale = 0.5f;

        [Tooltip("Displays the velocity texture on the screen. For debugging only.")]
        public bool preview;
    }
}
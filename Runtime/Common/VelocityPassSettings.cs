using System;
using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    [Serializable]
    public class VelocityPassSettings
    {
        public bool preview;
        public float pixelsPerUnit = 24;
        public RenderingLayerMask renderingLayerMask;
        public LayerMask layerMask;
        [Range(0.01f, 1f)] public float textureScale = 0.5f;
    }
}
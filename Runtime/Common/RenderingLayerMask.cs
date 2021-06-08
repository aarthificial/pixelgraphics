using System;
using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    [Serializable]
    public struct RenderingLayerMask
    {
        [SerializeField] private int mask;

        public static implicit operator uint(RenderingLayerMask mask) => (uint) mask.mask;

        public static implicit operator RenderingLayerMask(uint mask) => new RenderingLayerMask {mask = (int) mask};
    }
}
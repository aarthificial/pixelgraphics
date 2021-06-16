using System;
using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    [AddComponentMenu("PixelGraphics/Velocity Emitter")]
    [RequireComponent(typeof(SpriteRenderer))]
    public class VelocityEmitter : MonoBehaviour
    {
        public enum EmitterMode
        {
            Translation,
            Rigidbody,
            Rigidbody2D,
            Custom,
        }

        public Vector3 customVelocity;

        [SerializeField]
        [Tooltip("The emitter mode.")]
        private EmitterMode mode = EmitterMode.Translation;

        [SerializeField]
        private Rigidbody rb;

        [SerializeField]
        private Rigidbody2D rb2D;

        [SerializeField]
        [Tooltip("The maximum speed. Anything above that will be clamped.")]
        private float maxSpeed = 1;

        [SerializeField]
        [Tooltip(
            "Speed remapping.\nX-axis is the real speed (0 = 0, 1 = maxSpeed)\nY-axis is the speed passed to the velocity texture."
        )]
        private AnimationCurve remapping = AnimationCurve.Linear(0, 0, 1, 1);

        private SpriteRenderer _renderer;
        private Vector3 _lastPosition;
        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            Vector3 velocity;
            switch (mode)
            {
                case EmitterMode.Translation:
                    var position = transform.position;
                    velocity = (position - _lastPosition) / Time.deltaTime;
                    _lastPosition = position;
                    break;
                case EmitterMode.Rigidbody:
                    velocity = rb.velocity;
                    break;
                case EmitterMode.Rigidbody2D:
                    velocity = rb2D.velocity;
                    break;
                case EmitterMode.Custom:
                    velocity = customVelocity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            float speed = velocity.magnitude;
            if (speed > 0)
            {
                float scale = remapping.Evaluate(
                    Mathf.Clamp01(speed / maxSpeed)
                );

                velocity = velocity / speed * scale;
            }
            else
            {
                velocity = Vector3.zero;
            }

            if (_renderer.HasPropertyBlock())
                _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetVector(ShaderIds.PositionDelta, velocity);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
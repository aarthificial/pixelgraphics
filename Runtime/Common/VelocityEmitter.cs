using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class VelocityEmitter : MonoBehaviour
    {
        [SerializeField] private float maxVelocity = 1;
        [SerializeField] private AnimationCurve interpolation = AnimationCurve.Linear(0, 0, 1, 1);

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
            var position = transform.position;
            var delta = (position - _lastPosition) / Time.deltaTime;
            float velocity = delta.magnitude;

            if (velocity > 0)
            {
                float scale = interpolation.Evaluate(
                    Mathf.Clamp01(velocity / maxVelocity)
                );

                delta = delta / velocity * scale;
            }
            else
            {
                delta = Vector3.zero;
            }

            if (_renderer.HasPropertyBlock())
                _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetVector(ShaderIds.PositionDelta, delta);
            _renderer.SetPropertyBlock(_propertyBlock);

            _lastPosition = position;
        }
    }
}
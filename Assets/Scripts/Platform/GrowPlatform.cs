using DG.Tweening;
using UnityEngine;

namespace Platform
{
    public class GrowPlatform : MonoBehaviour
    {
        [SerializeField] private Transform platform;
        [SerializeField] private float targetHeight = 5f;
        [SerializeField] private float duration = 1f;

        private float originY;
        private Tween tween;

        private void Start()
        {
            if (platform != null)
                originY = platform.position.y;
        }

        public void Grow()
        {
            if (platform == null) return;
            tween?.Kill();
            tween = platform.DOMoveY(originY + targetHeight, duration).SetEase(Ease.OutCubic);
        }

        public void Shrink()
        {
            if (platform == null) return;
            tween?.Kill();
            tween = platform.DOMoveY(originY, duration).SetEase(Ease.InCubic);
        }

        private void OnDestroy()
        {
            tween?.Kill();
        }
    }
}

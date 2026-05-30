using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Platform
{
    /// <summary>
    /// 竖直升降平台：提供 Grow/Shrink 控制，并在动画结束时触发事件，
    /// 便于串联其他机关（如横向移动平台）。
    /// </summary>
    public class GrowPlatform : MonoBehaviour
    {
        [SerializeField] private Transform platform;         // 实际移动的平台部分
        [SerializeField] private float targetHeight = 5f;    // 升起高度（相对起始位置）
        [SerializeField] private float duration = 1f;        // 动画时长

        [Header("动画完成事件")]
        [Tooltip("平台完全升起后触发")]
        public UnityEvent OnGrowComplete;

        [Tooltip("平台完全降下后触发")]
        public UnityEvent OnShrinkComplete;

        private float originY;
        private Tween tween;

        private void Start()
        {
            if (platform != null)
                originY = platform.position.y;
        }

        /// <summary>
        /// 将平台升起到目标高度。
        /// </summary>
        public void Grow()
        {
            if (platform == null) return;
            tween?.Kill();
            tween = platform.DOMoveY(originY + targetHeight, duration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => OnGrowComplete?.Invoke());
        }

        /// <summary>
        /// 将平台降回原位。
        /// </summary>
        public void Shrink()
        {
            if (platform == null) return;
            tween?.Kill();
            tween = platform.DOMoveY(originY, duration)
                .SetEase(Ease.InCubic)
                .OnComplete(() => OnShrinkComplete?.Invoke());
        }

        private void OnDestroy()
        {
            tween?.Kill();
        }
    }
}
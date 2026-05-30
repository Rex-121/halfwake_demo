using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Platform
{
    /// <summary>
    /// 压力板：支持蘑菇单向踩踏、释放延迟（防止快速跳开误触发释放）。
    /// 同时保留 OnActivationChanged（供 Door）和 OnPressed/OnReleased（供 Inspector）。
    /// </summary>
    public class PressurePlate : MonoBehaviour
    {
        public event Action<PressurePlate, bool> OnActivationChanged;

        [Header("触发设置")]
        [SerializeField] private string[] activatorTags = { "Player", "Clone" };
        [SerializeField] private bool onlyTriggerFromAbove = false;

        [Header("释放延迟")]
        [Tooltip("角色离开后等待此秒数才真正释放。若期间再次进入则取消释放。")]
        [SerializeField] private float releaseDelay = 0.3f;   // 可调，例如 0.3 秒

        [Header("动画")]
        [SerializeField] private float pressDuration = 0.3f;
        [SerializeField] private Transform animationTransform;

        [Header("事件")]
        public UnityEvent OnPressed;
        public UnityEvent OnReleased;

        private bool wasActivated;
        private Vector3 originPos;
        private Tween tween;
        private Coroutine releaseCoroutine;   // 延迟释放协程

        private void Start()
        {
            originPos = transform.position;
            if (animationTransform == null)
                animationTransform = transform;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidActivator(other)) return;

            // 蘑菇模式方向检查
            if (onlyTriggerFromAbove)
            {
                Rigidbody2D otherRb = other.attachedRigidbody;
                if (otherRb != null && otherRb.velocity.y > 0.1f)
                    return;
            }

            // 如果正在等待释放，取消释放
            if (releaseCoroutine != null)
            {
                StopCoroutine(releaseCoroutine);
                releaseCoroutine = null;
            }

            // 若之前未激活，则激活
            if (!wasActivated)
            {
                Activate();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!wasActivated || !IsValidActivator(other)) return;

            // 启动延迟释放
            if (releaseCoroutine != null) StopCoroutine(releaseCoroutine);
            releaseCoroutine = StartCoroutine(DelayedDeactivate());
        }

        private IEnumerator DelayedDeactivate()
        {
            yield return new WaitForSeconds(releaseDelay);
            Deactivate();
            releaseCoroutine = null;
        }

        private void Activate()
        {
            wasActivated = true;

            tween?.Kill();
            float pressOffset = animationTransform.lossyScale.y / 2f;
            tween = animationTransform.DOMoveY(originPos.y - pressOffset, pressDuration)
                .SetEase(Ease.InOutCubic);

            OnActivationChanged?.Invoke(this, true);
            OnPressed?.Invoke();
        }

        private void Deactivate()
        {
            if (!wasActivated) return;
            wasActivated = false;

            tween?.Kill();
            tween = animationTransform.DOMoveY(originPos.y, pressDuration)
                .SetEase(Ease.InOutCubic);

            OnActivationChanged?.Invoke(this, false);
            OnReleased?.Invoke();
        }

        private bool IsValidActivator(Collider2D col)
        {
            GameObject root = col.attachedRigidbody != null
                ? col.attachedRigidbody.gameObject
                : col.gameObject;

            foreach (string tag in activatorTags)
            {
                if (root.CompareTag(tag))
                    return true;
            }
            return false;
        }

        private void OnDisable()
        {
            if (releaseCoroutine != null) StopCoroutine(releaseCoroutine);
            if (wasActivated)
            {
                wasActivated = false;
                OnActivationChanged?.Invoke(this, false);
                OnReleased?.Invoke();
            }
        }
    }
}
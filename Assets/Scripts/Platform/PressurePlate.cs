using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Platform
{
    /// <summary>
    /// 通用压力板：
    /// - 通过 OnActivationChanged 事件（C# 事件）通知代码订阅者（如 Door）
    /// - 通过 OnPressed / OnReleased（UnityEvent）在 Inspector 中绑定任意响应
    /// - 按压动画与去重逻辑保持不变
    /// </summary>
    public class PressurePlate : MonoBehaviour
    {
        // 传统 C# 事件（供 Door 等脚本使用）
        public event Action<PressurePlate, bool> OnActivationChanged;

        [Header("触发设置")]
        [SerializeField] private string[] activatorTags = { "Player", "Clone" };

        [Header("动画")]
        [SerializeField] private float pressDuration = 0.3f;
        [SerializeField] private Transform animationTransform;

        [Header("Inspector 事件")]
        public UnityEvent OnPressed;
        public UnityEvent OnReleased;

        private bool wasActivated;
        private Vector3 originPos;
        private Tween tween;

        private void Start()
        {
            originPos = transform.position;
            if (animationTransform == null)
                animationTransform = transform;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (wasActivated || !IsValidActivator(other))
                return;

            wasActivated = true;
            Debug.Log($"压力板 {gameObject.name} 激活");

            // 动画
            tween?.Kill();
            float pressOffset = animationTransform.lossyScale.y / 2f;
            tween = animationTransform.DOMoveY(originPos.y - pressOffset, pressDuration)
                .SetEase(Ease.InOutCubic);

            // 通知所有订阅者
            OnActivationChanged?.Invoke(this, true);
            OnPressed?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!wasActivated || !IsValidActivator(other))
                return;

            wasActivated = false;
            Debug.Log($"压力板 {gameObject.name} 释放");

            // 弹起动画
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
            if (wasActivated)
            {
                wasActivated = false;
                OnActivationChanged?.Invoke(this, false);
                OnReleased?.Invoke();
            }
        }
    }
}
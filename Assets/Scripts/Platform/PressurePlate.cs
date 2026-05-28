using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Platform
{
    public class PressurePlate : MonoBehaviour
    {
        public event Action<PressurePlate, bool> OnActivationChanged;

        [SerializeField] private string[] activatorTags = { "Player", "Clone" };
        [SerializeField] private float pressDuration = 0.3f;

        private bool wasActivated;
        private Vector3 originPos;
        private Tween tween;
        
        
        public GrowPlatform growPlatform;

        [ShowInInspector, SerializeField]
        private Transform animationTransform;
        private void Start()
        {
            originPos = transform.position;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (wasActivated || !IsValidActivator(other)) return;
            wasActivated = true;
            Debug.Log($"压力板 {gameObject.name} 被激活");
            tween?.Kill();
            tween = animationTransform.DOMoveY(originPos.y - animationTransform.lossyScale.y / 2f, pressDuration)
                .SetEase(Ease.InOutCubic);
            OnActivationChanged?.Invoke(this, true);
            if (growPlatform != null) growPlatform.Grow();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!wasActivated || !IsValidActivator(other)) return;
            wasActivated = false;
            Debug.Log($"压力板 {gameObject.name} 取消激活");
            tween?.Kill();
            tween = animationTransform.DOMoveY(originPos.y, pressDuration)
                .SetEase(Ease.InOutCubic);
            OnActivationChanged?.Invoke(this, false);
            if (growPlatform != null) growPlatform.Shrink();
        }

        private bool IsValidActivator(Collider2D col)
        {
            var go = col.attachedRigidbody != null ? col.attachedRigidbody.gameObject : col.gameObject;
            foreach (string tag in activatorTags)
            {
                if (go.CompareTag(tag))
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
            }
        }

        private void OnDestroy()
        {
            tween?.Kill();
        }
    }
}

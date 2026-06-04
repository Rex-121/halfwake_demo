using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Platform
{
    public class 压力板: SerializedMonoBehaviour
    {

        [SerializeField] private LayerMask activatorTags;
        [ShowInInspector]
        private bool _onlyTriggerFromAbove = true;

        [Header("动画")]
        [SerializeField] private float pressDuration = 0.3f;
        [SerializeField] private Transform animationTransform;

        [Header("音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pressedClip;
        [SerializeField] private AudioClip releasedClip;
        [SerializeField, Range(0f, 1f)] private float audioVolume = 1f;

        private bool _wasActivated;
        private Vector3 originPos;
        private Tween _tween;

        [HideInInspector]
        public BehaviorSubject<bool> isActive;

        private void Awake()
        {
            isActive = new(false);
        }

        private void Start()
        {
            if (animationTransform == null)
                animationTransform = transform;
            originPos = animationTransform.position;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidActivator(other)) return;

            // 蘑菇模式方向检查
            if (_onlyTriggerFromAbove)
            {
                Rigidbody2D otherRb = other.attachedRigidbody;
                if (otherRb != null && otherRb.velocity.y > 0.1f)
                    return;
            }

            d++;
            // 若之前未激活，则激活
            if (!_wasActivated)
            {
                Activate();
            }
        }

        public int d = 0;

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!_wasActivated || !IsValidActivator(other)) return;

            d--;
            
            if (d > 0) return;
            
            if (!_wasActivated) return;
            _wasActivated = false;

            // isActive.OnNext(_wasActivated);
            StartCoroutine(DD());
            上升动画();
            PlayReleasedSound();
        }

        private IEnumerator DD()
        {
            yield return new WaitForSeconds(玩家配置.main.机制.压力板失效延迟);
            isActive.OnNext(_wasActivated);
        }

        private void Activate()
        {
            _wasActivated = true;
            isActive.OnNext(_wasActivated);
            下降动画();
            PlayPressedSound();
        }

        [Button]
        private void 下降动画()
        {
            _tween?.Kill();
            var pressOffset = animationTransform.lossyScale.y / 2f;
            _tween = animationTransform.DOMoveY(originPos.y - pressOffset, pressDuration)
                .SetEase(Ease.InOutCubic);
        }

        [Button]
        private void 上升动画()
        {
            _tween?.Kill();
            _tween = animationTransform.DOMoveY(originPos.y, pressDuration)
                .SetEase(Ease.InOutCubic);
        }

        public void PlayPressedSound()
        {
            PlaySound(pressedClip);
        }

        public void PlayReleasedSound()
        {
            PlaySound(releasedClip);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            var source = audioSource != null ? audioSource : GetComponent<AudioSource>();
            if (source == null) return;

            source.PlayOneShot(clip, audioVolume);
        }

        private bool IsValidActivator(Collider2D col)
        {
            var root = col.attachedRigidbody != null? col.attachedRigidbody.gameObject: col.gameObject;                                                  
            return ((1 << root.layer) & activatorTags) != 0;
        }

        private void OnDisable()
        {
            _wasActivated = false;
            _tween?.Kill();
        }
        
    }
}

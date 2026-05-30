using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Platform
{
    public class 平台: SerializedMonoBehaviour
    {
        
        
        [SerializeField]
        public List<DOTweenAnimation> animations;


        [SerializeField] public List<压力板> 压力板s;
        
        [Button]
        public void Play()
        {
            var seq = DOTween.Sequence().SetAutoKill();
            foreach (var anim in animations)
            {
                seq.Append(anim.tween);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Platform
{
    public class 平台 : SerializedMonoBehaviour
    {
        [SerializeField] private List<压力板> 压力板s;
        [SerializeField] private 藤蔓茎 茎;
        [SerializeField] private 藤蔓头 头;

        private Coroutine _seq;

        public int latest;
        private void Start()
        {
            压力板s.Select((plate, i) => plate.isActive)
                .CombineLatest()
                .DistinctUntilChanged()
                .Subscribe(states =>
                {
                    var 踩中数量 = states.Count(x => x);
                    switch (踩中数量)
                    {
                        case 0:
                            if (_seq != null) StopCoroutine(_seq);
                            _seq = StartCoroutine(收缩序列());
                            break;
                        case 1:
                            if (_seq != null) StopCoroutine(_seq);
                            if (latest == 2)
                            {
                                _seq = StartCoroutine(展开序列());
                            }
                            else
                            {
                                _seq = StartCoroutine(展开序列(1));
                            }
                            break;
                        case 2:
                            if (_seq != null) StopCoroutine(_seq);
                            _seq = StartCoroutine(展开序列());
                            break;
                    }

                    latest = 踩中数量;
                })
                .AddTo(this);
        }
        
        private IEnumerator 展开序列(int c)
        {
            yield return StartCoroutine(茎.MoveUp());
            if (c < 2) yield break;
            yield return StartCoroutine(头.MoveLeft());
        }
        private IEnumerator 展开序列()
        {
            yield return StartCoroutine(茎.MoveUp());
            yield return StartCoroutine(头.MoveLeft());
        }

        private IEnumerator 收缩序列()
        {
            yield return StartCoroutine(头.MoveRight());
            yield return StartCoroutine(茎.MoveDown());
        }
    }
}

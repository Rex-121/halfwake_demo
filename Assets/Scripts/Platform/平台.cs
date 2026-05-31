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
    public class 平台 : SerializedMonoBehaviour
    {
        [SerializeField] public List<DOTweenAnimation> animations;


        [SerializeField] public List<压力板> 压力板s;

        private Stage zz = Stage.Clear;

        enum Stage
        {
            Clear, One, Two
        }
        private void Start()
        {
            压力板s.Select((plate, i) => plate.isActive)
                .CombineLatest()
                .Subscribe(states =>
                {
                    int activeCount = states.Count(x => x);
                    Debug.Log($"activeCount: {activeCount}");
                    // UpdateAnimation(activeCount);
                    // K(activeCount);
                    switch (activeCount)
                    {
                        case 0:
                            Z(Stage.Clear);
                            break;
                        case 1:
                            Z(Stage.One);
                            break;
                        case 2:
                            Z(Stage.Two);
                            break;
                    }

                    latest = activeCount;
                })
                .AddTo(this);
        }

        private void Z(Stage c)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);

            switch (c)
            {
                case Stage.Clear:
                    // 如果是0，说明要直接回到起点
                    if (zz != Stage.Clear)
                    {
                        _coroutine = StartCoroutine(播放倒叙(1));
                    }
                    else
                    {
                        _coroutine = StartCoroutine(播放倒叙(1));
                    }
                    break;
                case Stage.One:

                    if (zz == Stage.Clear)
                    {
                        _coroutine = StartCoroutine(PlayRange(0, 1, "up"));
                    }
                    
                    // if (zz == Stage.Two)
                    // {
                    //     _coroutine = StartCoroutine(播放倒叙(1));
                    // }
                    // else
                    // {
                    //     _coroutine = StartCoroutine(PlayRange(0, 1, "up"));
                    // }
                    
                    break;
                case Stage.Two:
                    if (zz == Stage.One)
                    {
                        _coroutine = StartCoroutine(PlayRange(0, 2, "up"));
                    }
                    else
                    {
                        _coroutine = StartCoroutine(PlayRange(0, 2, "up"));
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(c), c, null);
            }

            zz = c;

        }

        private void K(int c)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);

            switch (c)
            {
                case 0:
                    // 如果是0，说明要直接回到起点
                    _coroutine = StartCoroutine(播放倒叙(latest - 1));
                    break;
                case 1:
                    _coroutine = StartCoroutine(PlayRange(latest, c, "up"));
                    break;
                case 2:
                    break;
            }
            
            
            latest = c;

        }

        private Coroutine _coroutine;
        private int latest;

        private void UpdateAnimation(int activeCount)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);

            if (activeCount < latest)
            {
                _coroutine = StartCoroutine(PlayRangeX(0, 0, "down"));
            }
            else
            {
                _coroutine = StartCoroutine(PlayRange(latest, activeCount, "up"));
            }

            latest = activeCount;
        }

        private Tween c;

        private IEnumerator PlayRange(int from, int to, string id)
        {
            for (int i = from; i < to && i < animations.Count; i++)
            {
                
                //.DORestartById(id);
                // c?.Pause();
                c = animations[i].tween;
                if (c.IsComplete())
                {
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    
                    c.PlayForward();
                    yield return new WaitForSeconds(animations[i].duration);    
                }
            }
        }

        private IEnumerator 播放倒叙(int to)
        {
            var d = to;
            while (d >= 0)
            {
                // c?.Pause();
                c = animations[to].tween;
                c.PlayBackwards();
                
                yield return new WaitForSeconds(animations[to].duration);    
                
                d--;
                yield return 播放倒叙(d);
            }
        }
        
        // private IEnumerator 播放GGG倒叙(int to)
        // {
        //     yield return 播放倒叙(to);
        //     yield return 
        // }
        
        private IEnumerator PlayRangeX(int from, int to, string id)
        {
            c?.Pause();
            c = animations[0].tween;
            c.PlayBackwards();
            yield return new WaitForSeconds(animations[0].duration);
        }
    }
}

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using UniRx;
// using UnityEngine;
//
// namespace Platform
// {
//     public class 平台 : SerializedMonoBehaviour
//     {
//         [SerializeField] public List<DOTweenAnimation> animations;
//
//
//         [SerializeField] public List<压力板> 压力板s;
//
//         private Stage zz = Stage.Clear;
//
//         enum Stage
//         {
//             Clear, One, Two
//         }
//         private void Start()
//         {
//             压力板s.Select((plate, i) => plate.isActive)
//                 .CombineLatest()
//                 .Subscribe(states =>
//                 {
//                     int activeCount = states.Count(x => x);
//                     Debug.Log($"activeCount: {activeCount}");
//                     // UpdateAnimation(activeCount);
//                     // K(activeCount);
//                     switch (activeCount)
//                     {
//                         case 0:
//                             Z(Stage.Clear);
//                             break;
//                         case 1:
//                             Z(Stage.One);
//                             break;
//                         case 2:
//                             Z(Stage.Two);
//                             break;
//                     }
//
//                     latest = activeCount;
//                 })
//                 .AddTo(this);
//         }
//
//         private void Z(Stage c)
//         {
//             if (_coroutine != null) StopCoroutine(_coroutine);
//
//             switch (c)
//             {
//                 case Stage.Clear:
//                     // 如果是0，说明要直接回到起点
//                     _coroutine = StartCoroutine(播放倒叙(latest - 1));
//                     
//                     // if (zz == Stage.Two)
//                     // {
//                     //     _coroutine = StartCoroutine(播放倒叙(2));
//                     // }
//                     // else
//                     // {
//                     //     _coroutine = StartCoroutine(播放倒叙(1));
//                     // }
//                     
//                     break;
//                 case Stage.One:
//
//                     if (zz == Stage.Two)
//                     {
//                         _coroutine = StartCoroutine(播放倒叙(1));
//                     }
//                     else
//                     {
//                         _coroutine = StartCoroutine(PlayRange(0, 1, "up"));
//                     }
//                     
//                     break;
//                 case Stage.Two:
//                     if (zz == Stage.One)
//                     {
//                         _coroutine = StartCoroutine(PlayRange(0, 2, "up"));
//                     }
//                     else
//                     {
//                         _coroutine = StartCoroutine(PlayRange(0, 2, "up"));
//                     }
//                     
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(c), c, null);
//             }
//
//             zz = c;
//
//         }
//
//         private void K(int c)
//         {
//             if (_coroutine != null) StopCoroutine(_coroutine);
//
//             switch (c)
//             {
//                 case 0:
//                     // 如果是0，说明要直接回到起点
//                     _coroutine = StartCoroutine(播放倒叙(latest - 1));
//                     break;
//                 case 1:
//                     _coroutine = StartCoroutine(PlayRange(latest, c, "up"));
//                     break;
//                 case 2:
//                     break;
//             }
//             
//             
//             latest = c;
//
//         }
//
//         private Coroutine _coroutine;
//         private int latest;
//
//         private void UpdateAnimation(int activeCount)
//         {
//             if (_coroutine != null) StopCoroutine(_coroutine);
//
//             if (activeCount < latest)
//             {
//                 _coroutine = StartCoroutine(PlayRangeX(0, 0, "down"));
//             }
//             else
//             {
//                 _coroutine = StartCoroutine(PlayRange(latest, activeCount, "up"));
//             }
//
//             latest = activeCount;
//         }
//
//         private Tween c;
//
//         private IEnumerator PlayRange(int from, int to, string id)
//         {
//             for (int i = from; i < to && i < animations.Count; i++)
//             {
//                 c?.Pause();
//                 c = animations[i].tween;
//                 c.PlayForward(); //.DORestartById(id);
//                 yield return new WaitForSeconds(animations[i].duration);
//             }
//         }
//
//         private IEnumerator 播放倒叙(int to)
//         {
//             var d = to;
//             while (d >= 0)
//             {
//                 c?.Pause();
//                 c = animations[to].tween;
//                 c.PlayBackwards();
//                 yield return new WaitForSeconds(animations[to].duration);
//                 d--;
//             }
//         }
//         
//         // private IEnumerator 播放GGG倒叙(int to)
//         // {
//         //     yield return 播放倒叙(to);
//         //     yield return 
//         // }
//         
//         private IEnumerator PlayRangeX(int from, int to, string id)
//         {
//             c?.Pause();
//             c = animations[0].tween;
//             c.PlayBackwards();
//             yield return new WaitForSeconds(animations[0].duration);
//         }
//     }
// }
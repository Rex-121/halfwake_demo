using System;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;

namespace Record
{
    public class RecordController : SerializedMonoBehaviour
    {
        public static RecordController main;
        public IObservable<bool> IsRecording => onIsRecording;
        public IObservable<Unit> OnSpawnPressed => onSpawnPressed;

        private Subject<bool> onIsRecording = new Subject<bool>();
        private Subject<Unit> onSpawnPressed = new Subject<Unit>();
        public bool isRecording { get; private set; }

        private void Awake()
        {
            if (main == null)
            {
                main = this;
                Make();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Make()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.R))
                .Subscribe(_ =>
                {
                    isRecording = !isRecording;
                    onIsRecording.OnNext(isRecording);
                })
                .AddTo(this);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.X))
                .Subscribe(_ => onSpawnPressed.OnNext(Unit.Default))
                .AddTo(this);
        }
        
    }
}

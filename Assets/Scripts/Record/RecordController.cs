
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
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isRecording = !isRecording;
                onIsRecording.OnNext(isRecording);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                onSpawnPressed.OnNext(Unit.Default);
            }
        }

        /// <summary>
        /// 外部调用停止录制
        /// </summary>
        public void StopRecording()
        {
            isRecording = false;
            onIsRecording.OnNext(false);
        }
    }
}

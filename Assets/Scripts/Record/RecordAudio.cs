using UniRx;
using UnityEngine;

namespace Record
{
    public class RecordAudio : MonoBehaviour
    {
        [Header("音源")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        [Header("录制音效")]
        [SerializeField] private AudioClip recordStartClip;
        [SerializeField] private AudioClip recordingLoopClip;
        [SerializeField] private AudioClip recordStopClip;

        [Header("回放音效")]
        [SerializeField] private AudioClip replayClip;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            var controller = RecordController.main;
            if (controller == null) return;

            controller.IsRecording
                .Subscribe(OnRecordingChanged)
                .AddTo(disposables);

            controller.OnSpawnPressed
                .Subscribe(_ => PlayReplaySound())
                .AddTo(disposables);
        }

        public void PlayRecordStartSound()
        {
            PlayOneShot(recordStartClip);
        }

        public void PlayRecordStopSound()
        {
            PlayOneShot(recordStopClip);
        }

        public void PlayReplaySound()
        {
            PlayOneShot(replayClip);
        }

        public void StartRecordingLoopSound()
        {
            if (audioSource == null || recordingLoopClip == null) return;

            audioSource.clip = recordingLoopClip;
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.Play();
        }

        public void StopRecordingLoopSound()
        {
            if (audioSource == null || audioSource.clip != recordingLoopClip) return;

            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }

        private void OnRecordingChanged(bool isRecording)
        {
            if (isRecording)
            {
                PlayRecordStartSound();
                StartRecordingLoopSound();
            }
            else
            {
                StopRecordingLoopSound();
                PlayRecordStopSound();
            }
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (clip == null) return;

            var source = audioSource != null ? audioSource : GetComponent<AudioSource>();
            if (source == null) return;

            source.PlayOneShot(clip, volume);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}

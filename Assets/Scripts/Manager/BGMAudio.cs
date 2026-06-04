using UnityEngine;

namespace Manager
{
    [RequireComponent(typeof(AudioSource))]
    public class BGMAudio : MonoBehaviour
    {
        [Header("BGM")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip bgmClip;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool loop = true;
        [SerializeField] private bool dontDestroyOnLoad = true;

        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            ApplySettings();

            if (playOnAwake)
                PlayBGM();
        }

        public void PlayBGM()
        {
            if (audioSource == null || bgmClip == null) return;

            audioSource.clip = bgmClip;
            audioSource.volume = volume;
            audioSource.loop = loop;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        public void StopBGM()
        {
            if (audioSource == null) return;

            audioSource.Stop();
        }

        public void PauseBGM()
        {
            if (audioSource == null) return;

            audioSource.Pause();
        }

        public void ResumeBGM()
        {
            if (audioSource == null) return;

            audioSource.UnPause();
        }

        public void SetVolume(float value)
        {
            volume = Mathf.Clamp01(value);

            if (audioSource != null)
                audioSource.volume = volume;
        }

        private void ApplySettings()
        {
            if (audioSource == null) return;

            audioSource.playOnAwake = false;
            audioSource.loop = loop;
            audioSource.volume = volume;
        }
    }
}

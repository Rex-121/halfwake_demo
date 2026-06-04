using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        [Header("音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clickClip;
        [SerializeField] private AudioClip hoverClip;
        [SerializeField] private AudioClip pressedClip;
        [SerializeField, Range(0f, 1f)] private float audioVolume = 1f;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (button == null)
                button = GetComponent<Button>();

            button.onClick.AddListener(PlayClickSound);
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(PlayClickSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PlayPressedSound();
        }

        public void PlayClickSound()
        {
            PlaySound(clickClip);
        }

        public void PlayHoverSound()
        {
            PlaySound(hoverClip);
        }

        public void PlayPressedSound()
        {
            PlaySound(pressedClip);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            var source = audioSource != null ? audioSource : GetComponent<AudioSource>();
            if (source == null) return;

            source.PlayOneShot(clip, audioVolume);
        }
    }
}

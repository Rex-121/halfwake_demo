using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FootstepAudio : MonoBehaviour
    {
        [Header("音源")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        [Header("脚步声")]
        [SerializeField] private AudioClip[] footstepClips;
        [SerializeField] private float stepInterval = 0.35f;
        [SerializeField] private float minMoveSpeed = 0.1f;

        [Header("接地检测")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f;

        private Rigidbody2D rb;
        private float stepTimer;
        private int clipIndex;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (groundCheck == null)
                groundCheck = transform.Find("GroundCheck");
        }

        private void Update()
        {
            if (!ShouldPlayFootstep())
            {
                stepTimer = stepInterval;
                return;
            }

            stepTimer += Time.deltaTime;
            if (stepTimer < stepInterval) return;

            PlayFootstep();
            stepTimer = 0f;
        }

        public void PlayFootstep()
        {
            if (audioSource == null || footstepClips == null || footstepClips.Length == 0) return;

            AudioClip clip = footstepClips[clipIndex % footstepClips.Length];
            clipIndex++;

            if (clip == null) return;

            audioSource.PlayOneShot(clip, volume);
        }

        private bool ShouldPlayFootstep()
        {
            if (rb == null) return false;
            if (Mathf.Abs(rb.velocity.x) < minMoveSpeed) return false;
            if (groundCheck == null) return true;

            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }
}

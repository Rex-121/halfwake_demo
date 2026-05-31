using Player;
using UnityEngine;

namespace Record
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CloneController : MonoBehaviour
    {
        private Record recording;

        private Rigidbody2D rb;
        private Transform groundCheck;
        private Vector3 baseScale;
        private int currentFrameIndex;
        private float playbackStartTime;
        private bool isPlaying;

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Sprite[] jumpSprites;
        private float jumpFrameInterval = 0.15f;
        private float landFrameInterval = 0.1f;
        private bool wasInAir;
        private float airTimer;
        private float landTimer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(Record data)
        {
            recording = data;
            CopyPlayerAppearance();
            baseScale = transform.localScale;
            CreateGroundCheck();

            currentFrameIndex = 0;
            playbackStartTime = Time.time;
            isPlaying = true;

            rb.velocity = recording.startVelocity;
        }

        private void CopyPlayerAppearance()
        {
            var playerAvatar = GameObject.FindWithTag("Player")?.GetComponentInChildren<PlayerAnimation>();
            if (playerAvatar == null) return;

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = playerAvatar.GetComponent<SpriteRenderer>().sprite;
                sr.color = new Color(1f, 1f, 1f, 0.6f);
            }

            var playerCol = playerAvatar.GetComponent<BoxCollider2D>();
            var col = GetComponent<BoxCollider2D>();
            if (col != null && playerCol != null)
            {
                col.offset = playerCol.offset;
                col.size = playerCol.size;
            }

            transform.localScale = playerAvatar.transform.parent.localScale;

            // 复制Animator
            var playerAnimator = playerAvatar.GetComponent<PlayerAnimation>();
            if (playerAnimator != null && playerAnimator.GetComponent<Animator>().runtimeAnimatorController != null)
            {
                animator = GetComponent<Animator>() ?? gameObject.AddComponent<Animator>();
                animator.runtimeAnimatorController = playerAnimator.GetComponent<Animator>().runtimeAnimatorController;
            }

            // 复制跳跃精灵
            jumpSprites = playerAvatar.JumpSprites;
        }

        private void CreateGroundCheck()
        {
            groundCheck = new GameObject("GroundCheck").transform;
            groundCheck.SetParent(transform);
            groundCheck.localPosition = new Vector3(0, -0.5f, 0);
        }

        private void FixedUpdate()
        {
            if (!isPlaying || recording == null || recording.frames.Count == 0) return;

            float elapsed = Time.time - playbackStartTime;

            if (elapsed > recording.totalDuration)
            {
                isPlaying = false;
                Destroy(gameObject);
                return;
            }

            while (currentFrameIndex < recording.frames.Count - 1
                   && recording.frames[currentFrameIndex + 1].time <= elapsed)
            {
                currentFrameIndex++;
            }

            var frame = recording.frames[currentFrameIndex];
            ApplyFrame(rb, frame);

            if (frame.inputX != 0)
                transform.localScale = new Vector3(-Mathf.Sign(frame.inputX) * baseScale.x, baseScale.y, baseScale.z);
        }

        private void Update()
        {
            if (rb == null) return;

            bool inAir = Mathf.Abs(rb.velocity.y) > 0.1f;

            if (inAir)
            {
                if (animator != null) animator.enabled = false;
                PlayAirFrames();
            }
            else if (wasInAir)
            {
                if (animator != null) animator.enabled = false;
                PlayLandFrames();
            }
            else
            {
                if (animator != null)
                {
                    animator.enabled = true;
                    animator.SetBool("walking", Mathf.Abs(rb.velocity.x) > 0.1f);
                }
            }

            wasInAir = inAir;
        }

        private void PlayAirFrames()
        {
            airTimer += Time.deltaTime;
            int frame = Mathf.Min((int)(airTimer / jumpFrameInterval), 2);
            if (jumpSprites != null && frame < jumpSprites.Length)
                spriteRenderer.sprite = jumpSprites[frame];
        }

        private void PlayLandFrames()
        {
            landTimer += Time.deltaTime;
            int frame = Mathf.Min((int)(landTimer / landFrameInterval) + 3, jumpSprites.Length - 1);
            if (jumpSprites != null && frame < jumpSprites.Length)
                spriteRenderer.sprite = jumpSprites[frame];

            if (landTimer >= landFrameInterval * 2)
            {
                landTimer = 0;
                airTimer = 0;
                wasInAir = false;
            }
        }

        public void ApplyFrame(Rigidbody2D rb, RecordedFrame frame)
        {
            rb.velocity = new Vector2(frame.inputX * frame.moveSpeed, rb.velocity.y);
            if (frame.jump)
                rb.velocity = new Vector2(rb.velocity.x, frame.jumpForce);
        }

        private void OnDestroy()
        {
            if (groundCheck != null)
            {
                Destroy(groundCheck.gameObject);
            }
        }
    }
}

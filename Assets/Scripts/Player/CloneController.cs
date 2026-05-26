using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CloneController : MonoBehaviour
    {
        private RecordingData recording;
        private float moveSpeed;
        private float jumpForce;
        private LayerMask groundLayer;
        private float groundCheckRadius;

        private Rigidbody2D rb;
        private Transform groundCheck;
        private int currentFrameIndex;
        private float playbackStartTime;
        private bool isPlaying;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void Initialize(RecordingData data, float speed, float jump, LayerMask layer, float radius)
        {
            recording = data;
            moveSpeed = speed;
            jumpForce = jump;
            groundLayer = layer;
            groundCheckRadius = radius;

            CreateGroundCheck();

            currentFrameIndex = 0;
            playbackStartTime = Time.time;
            isPlaying = true;

            rb.velocity = recording.startVelocity;
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
                return;
            }

            while (currentFrameIndex < recording.frames.Count - 1 && recording.frames[currentFrameIndex + 1].time <= elapsed)
            {
                currentFrameIndex++;
            }

            FrameData currentFrame = recording.frames[currentFrameIndex];

            rb.velocity = new Vector2(currentFrame.inputX * moveSpeed, rb.velocity.y);

            if (currentFrame.jump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
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
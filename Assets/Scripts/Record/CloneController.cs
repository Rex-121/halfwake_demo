using UnityEngine;

namespace Record
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CloneController : MonoBehaviour
    {
        private Record recording;

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

        public void Initialize(Record data)
        {
            recording = data;
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
                transform.localScale = new Vector3(-Mathf.Sign(frame.inputX) * 3f, 3f, 3f);
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
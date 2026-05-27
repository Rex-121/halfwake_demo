using UnityEngine;
using System.Collections.Generic;

namespace Record
{
    public struct FrameData
    {
        public float inputX;
        public bool jump;
    }

    public struct RecordedFrame
    {
        public float time;
        public FrameData input;
        public Vector2 position;
        public Vector2 velocity;
        public bool isGrounded;
    }

    public class Record
    {
        public List<RecordedFrame> frames { get; private set; } = new List<RecordedFrame>();
        public Vector2 startPosition;
        public Vector2 startVelocity;
        public float totalDuration;
        public bool IsRecording => isRecording;

        private bool isRecording;
        private float recordingStartTime;
        private float maxDuration;

        public Record(float maxDuration)
        {
            this.maxDuration = maxDuration;
        }

        public void StartRecording(Vector2 position, Vector2 velocity)
        {
            isRecording = true;
            recordingStartTime = Time.time;
            frames.Clear();
            totalDuration = 0f;
            startPosition = position;
            startVelocity = velocity;
        }

        public void StopRecording()
        {
            isRecording = false;
        }

        public void RecordFrame(FrameData input, Vector2 position, Vector2 velocity, bool isGrounded)
        {
            if (!isRecording) return;

            float elapsed = Time.time - recordingStartTime;
            if (elapsed > maxDuration)
            {
                StopRecording();
                return;
            }

            var frame = new RecordedFrame
            {
                time = elapsed,
                input = input,
                position = position,
                velocity = velocity,
                isGrounded = isGrounded
            };

            frames.Add(frame);
            if (elapsed > totalDuration)
                totalDuration = elapsed;
        }
    }

    public interface IRecordable
    {
        Record CurrentRecord { get; }
        void StartRecording();
        void StopRecording();
    }

    public static class FrameDriver
    {
        public static void ApplyFrame(Rigidbody2D rb, FrameData frame, float moveSpeed, float jumpForce)
        {
            rb.velocity = new Vector2(frame.inputX * moveSpeed, rb.velocity.y);
            if (frame.jump)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}

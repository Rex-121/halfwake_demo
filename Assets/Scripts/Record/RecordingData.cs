using UnityEngine;
using System.Collections.Generic;

namespace Record
{
    public struct FrameData //输入数据
    {
        public float inputX;
        public bool jump;
    }

    public struct RecordedFrame //录制帧 = 输入 + 状态
    {
        public float time;
        public FrameData input;
        public Vector2 position;
        public Vector2 velocity;
        public bool isGrounded;
    }

    public interface IRecordable
    {
        RecordingData CurrentRecording { get; }
        void StartRecording();
        void StopRecording();
    }

    public class RecordingData
    {
        public List<RecordedFrame> frames = new List<RecordedFrame>();
        public Vector2 startPosition;
        public Vector2 startVelocity;
        public float totalDuration;

        public void Clear()
        {
            frames.Clear();
            totalDuration = 0f;
        }

        public void AddFrame(RecordedFrame frame)
        {
            frames.Add(frame);
            if (frame.time > totalDuration)
                totalDuration = frame.time;
        }
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

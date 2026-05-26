using UnityEngine;
using System.Collections.Generic;

namespace Player
{
    public struct FrameData //帧数据结构体
    {
        public float time;
        public Vector2 position;
        public Vector2 velocity;
        public float inputX;
        public bool jump;
        public bool isGrounded;
    }

    public class RecordingData //录制数据类，用于存储每一帧的状态和输入
    {
        public List<FrameData> frames = new List<FrameData>();
        public Vector2 startPosition;
        public Vector2 startVelocity;
        public float totalDuration;

        public void Clear()
        {
            frames.Clear();
            totalDuration = 0f;
        }

        public void AddFrame(FrameData frame)
        {
            frames.Add(frame);
            if (frame.time > totalDuration)
                totalDuration = frame.time;
        }
    }
}
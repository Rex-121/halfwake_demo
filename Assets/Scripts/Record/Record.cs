using UnityEngine;
using System.Collections.Generic;

namespace Record
{
    public struct RecordedFrame
    {
        public float inputX;
        public bool jump;
        public float time;
    }

    public class Record
    {
        public List<RecordedFrame> frames { get; private set; } = new List<RecordedFrame>();
        public Vector2 startPosition;
        public Vector2 startVelocity;
        public float totalDuration;

        private float recordingStartTime;
        private float maxDuration;

        public bool recording;
        public Record(float maxDuration)
        {
            this.maxDuration = maxDuration;
        }

        public void StartRecording(Vector2 position, Vector2 velocity)
        {
            recordingStartTime = Time.time;
            frames.Clear();
            totalDuration = 0f;
            startPosition = position;
            startVelocity = velocity;
        }

        public void RecordFrame(RecordedFrame frame)
        {
            if (!recording) return;
            float elapsed = Time.time - recordingStartTime;
            frame.time = elapsed;
            if (elapsed > maxDuration) return;

            frames.Add(frame);
            if (elapsed > totalDuration)
                totalDuration = elapsed;
        }
    }

 
}

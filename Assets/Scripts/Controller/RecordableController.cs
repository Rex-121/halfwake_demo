using UnityEngine;
using UniRx;
using Record;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class RecordableController : MonoBehaviour
    {
        [SerializeField] protected GameObject clonePrefab;
        protected Record.Record record;
        protected Rigidbody2D rb;
        protected GameObject activeClone;
        protected GameObject recordingGhost;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            record = new Record.Record(玩家配置.main.maxRecordDuration);
        }

        protected virtual void Start()
        {
            SubscribeRecordController();
        }

        protected void SubscribeRecordController()
        {
            var rc = RecordController.main;
            rc.IsRecording
                .Subscribe(isRecording =>
                {
                    if (isRecording)
                        StartRecording();
                    else
                        StopRecording();
                })
                .AddTo(this);

            rc.OnSpawnPressed
                .Where(_ => record.frames.Count > 0)
                .Subscribe(_ => SpawnClone())
                .AddTo(this);
        }

        public virtual void StartRecording()
        {
            if (activeClone != null)
            {
                Destroy(activeClone);
                activeClone = null;
            }
            if (recordingGhost != null)
                Destroy(recordingGhost);

            record.recording = true;
            record.StartRecording(transform.position, rb.velocity);
            CreateRecordingGhost();
        }

        public virtual void StopRecording()
        {
            record.recording = false;
        }

        protected RecordedFrame ApplyFrame(Rigidbody2D rb, RecordedFrame frame)
        {
            rb.velocity = new Vector2(frame.inputX * frame.moveSpeed, rb.velocity.y);
            if (frame.jump)
                rb.velocity = new Vector2(rb.velocity.x, frame.jumpForce);
            return frame;
        }
        public void SpawnClone()
        {
            if (recordingGhost != null)
            {
                Destroy(recordingGhost);
                recordingGhost = null;
            }

            if (activeClone != null)
                Destroy(activeClone);

            if (clonePrefab == null)
                clonePrefab = CreateClonePrefab();

            activeClone = Instantiate(clonePrefab, record.startPosition, Quaternion.identity);
            var cloneController = activeClone.GetComponent<CloneController>();
            if (cloneController != null)
            {
                cloneController.Initialize(record);
            }
        }

        protected void CreateRecordingGhost()
        {
            recordingGhost = new GameObject("RecordingGhost");
            recordingGhost.transform.position = transform.position;
            recordingGhost.transform.localScale = transform.lossyScale;

            SpriteRenderer ghostSprite = recordingGhost.AddComponent<SpriteRenderer>();
            SpriteRenderer playerSprite = GetComponentInChildren<SpriteRenderer>();
            if (playerSprite != null)
            {
                ghostSprite.sprite = playerSprite.sprite;
                ghostSprite.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }

        protected virtual GameObject CreateClonePrefab()
        {
            return Resources.Load<GameObject>("Player/Clone");
        }
    }
}

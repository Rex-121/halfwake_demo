using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Record;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IRecordable
    {
        private float moveSpeed => 玩家配置.main.moveSpeed;
        private float jumpForce => 玩家配置.main.jumpForce;
        private float maxRecordDuration => 玩家配置.main.maxRecordDuration;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private GameObject clonePrefab;

        public RecordingData CurrentRecording { get; private set; } = new RecordingData();

        private Rigidbody2D rb;
        private bool isGrounded;
        private bool isRecording;
        private float recordingStartTime;
        private FrameData currentInput;
        private GameObject activeClone;
        private GameObject recordingGhost;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            //采集输入
            currentInput.inputX = Input.GetAxis("Horizontal");
            if (Input.GetButtonDown("Jump") && isGrounded)
                currentInput.jump = true;

            CheckGround();
            HandleRecordingInput();
        }

        private void FixedUpdate()
        {
            //用RF驱动移动
            FrameDriver.ApplyFrame(rb, currentInput, moveSpeed, jumpForce);
            //录制
            RecordFrame();
            //重置跳跃
            currentInput.jump = false;
        }

        private void CheckGround()
        {
            if (groundCheck == null) return;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        private void HandleRecordingInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!isRecording)
                    StartRecording();
                else
                    StopRecording();
            }

            if (Input.GetKeyDown(KeyCode.X) && CurrentRecording.frames.Count > 0)
            {
                SpawnClone();
            }
        }

        public void StartRecording()
        {
            if (activeClone != null)
            {
                Destroy(activeClone);
                activeClone = null;
            }

            if (recordingGhost != null)
            {
                Destroy(recordingGhost);
            }

            isRecording = true;
            recordingStartTime = Time.time;
            CurrentRecording.Clear();
            CurrentRecording.startPosition = transform.position;
            CurrentRecording.startVelocity = rb.velocity;

            CreateRecordingGhost();
            Debug.Log("录制开始");
        }

        public void StopRecording()
        {
            isRecording = false;
            Debug.Log("录制结束");
        }

        private void RecordFrame()
        {
            if (!isRecording) return;

            float elapsed = Time.time - recordingStartTime;
            if (elapsed > maxRecordDuration)
            {
                StopRecording();
                return;
            }

            RecordedFrame frame = new RecordedFrame
            {
                time = elapsed,
                input = currentInput,
                position = transform.position,
                velocity = rb.velocity,
                isGrounded = isGrounded
            };

            CurrentRecording.AddFrame(frame);
        }

        private void SpawnClone()
        {
            if (recordingGhost != null)
            {
                Destroy(recordingGhost);
                recordingGhost = null;
            }

            if (activeClone != null)
            {
                Destroy(activeClone);
            }

            if (clonePrefab == null)
            {
                clonePrefab = CreateClonePrefab();
            }

            activeClone = Instantiate(clonePrefab, CurrentRecording.startPosition, Quaternion.identity);
            var cloneController = activeClone.GetComponent<CloneController>();
            if (cloneController != null)
            {
                cloneController.Initialize(CurrentRecording, moveSpeed, jumpForce, groundLayer, groundCheckRadius);
            }
        }

        private void CreateRecordingGhost()
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

        private GameObject CreateClonePrefab()
        {
            return Resources.Load<GameObject>("Player/Clone");
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

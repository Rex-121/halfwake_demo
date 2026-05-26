using UnityEngine;
using System.Collections.Generic;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("移动")]
        [SerializeField] private float moveSpeed = 5f; //移动速度

        [Header("跳跃")]
        [SerializeField] private float jumpForce = 10f; //跳跃力度
        [SerializeField] private Transform groundCheck; //触地检测子对象
        [SerializeField] private LayerMask groundLayer; //地面Layer层
        [SerializeField] private float groundCheckRadius = 0.2f; //触底范围检测半径

        [Header("Recording")]
        [SerializeField] private float maxRecordDuration = 4f; //最大录制时间
        [SerializeField] private GameObject clonePrefab;//克隆预制体

        public RecordingData CurrentRecording { get; private set; } = new RecordingData();
        //录制数据
        public bool IsRecording => isRecording; //是否正在录制

        private Rigidbody2D rb;
        private bool isGrounded; //是否触地
        private bool isRecording; //是否正在录制
        private float recordingStartTime; //录制开始时间
        private float horizontalInput;
        private bool jumpPressed; //是否按下跳跃键
        private GameObject activeClone; //当前存在的克隆对象
        private GameObject recordingGhost;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            CheckGround();
            HandleJump();
            HandleRecordingInput();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            RecordFrame();
        }

        private void HandleMovement()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        private void HandleJump()
        {
            jumpPressed = Input.GetButtonDown("Jump");
            if (jumpPressed && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
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
                {
                    StartRecording();
                }
                else
                {
                    StopRecording();
                }
            }

            if (Input.GetKeyDown(KeyCode.X) && CurrentRecording.frames.Count > 0)
            {
                SpawnClone();
            }
        }

        private void StartRecording()
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

            CreateRecordingGhost();
        }

        private void StopRecording()
        {
            isRecording = false;
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

            FrameData frame = new FrameData
            {
                time = elapsed,
                position = transform.position,
                velocity = rb.velocity,
                inputX = horizontalInput,
                jump = jumpPressed,
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
            GameObject prefab = Resources.Load<GameObject>("Player/Clone");
            return prefab;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
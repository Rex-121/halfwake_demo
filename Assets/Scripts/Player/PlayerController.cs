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
        private GameObject recordingGhost; //录制时的半透明预览对象

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            //触地检测
            CheckGround();
            //跳跃处理
            HandleJump();
            //录制输入检测
            HandleRecordingInput();
        }

        private void FixedUpdate()
        {
            //移动检测
            HandleMovement();
            //录制帧数据
            RecordFrame();
            //重置跳跃状态，确保下一帧能捕捉新跳跃
            jumpPressed = false;
        }
        private void HandleMovement() //移动
        {
            horizontalInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
        private void HandleJump()  //跳跃
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpPressed = true;
            }
        }
        
        private void CheckGround()  //触地检测
        {
            if (groundCheck == null) return;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        private void HandleRecordingInput() //录制输入检测
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!isRecording)
                {
                    StartRecording(); //开始录制
                }
                else
                {
                    StopRecording();  //结束录制
                }
            }

            if (Input.GetKeyDown(KeyCode.X) && CurrentRecording.frames.Count > 0)
            {
                SpawnClone();
            }
        }

        private void StartRecording()  //开始录制
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

        private void StopRecording() //结束录制
        {
            isRecording = false;
            Debug.Log("录制结束");
        }

        private void RecordFrame() //录制帧数据
        {
            if (!isRecording) return;
            //计算录制时间，检测是否超过四秒
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

        private void SpawnClone()  //生成克隆体
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

        private void CreateRecordingGhost() //创建半透明克隆体
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

        private GameObject CreateClonePrefab() //获得克隆预制体
        {
            GameObject prefab = Resources.Load<GameObject>("Player/Clone");
            return prefab;
        }

        private void OnDrawGizmosSelected() //在编辑器中显示触地检测范围
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
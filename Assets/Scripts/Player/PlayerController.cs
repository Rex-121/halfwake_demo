using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Record;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        private float moveSpeed => 玩家配置.main.moveSpeed;
        private float jumpForce => 玩家配置.main.jumpForce;
        private float maxRecordDuration => 玩家配置.main.maxRecordDuration;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private GameObject clonePrefab;

        private Record.Record record;
        private Rigidbody2D rb;
        private bool isGrounded;
        private FrameData currentInput;
        private GameObject activeClone;
        private GameObject recordingGhost;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            record = new Record.Record(maxRecordDuration);
        }

        private void Update()
        {
            currentInput.inputX = Input.GetAxis("Horizontal");
            if (Input.GetButtonDown("Jump") && isGrounded)
                currentInput.jump = true;

            CheckGround();
            HandleRecordingInput();
        }

        private void FixedUpdate()
        {
            FrameDriver.ApplyFrame(rb, currentInput, moveSpeed, jumpForce);
            record.RecordFrame(currentInput, transform.position, rb.velocity, isGrounded);
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
                if (!record.IsRecording)
                {
                    if (activeClone != null)
                    {
                        Destroy(activeClone);
                        activeClone = null;
                    }
                    if (recordingGhost != null)
                        Destroy(recordingGhost);

                    record.StartRecording(transform.position, rb.velocity);
                    CreateRecordingGhost();
                }
                else
                {
                    record.StopRecording();
                }
            }

            if (Input.GetKeyDown(KeyCode.X) && record.frames.Count > 0)
            {
                SpawnClone();
            }
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

            activeClone = Instantiate(clonePrefab, record.startPosition, Quaternion.identity);
            var cloneController = activeClone.GetComponent<CloneController>();
            if (cloneController != null)
            {
                cloneController.Initialize(record, moveSpeed, jumpForce, groundLayer, groundCheckRadius);
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

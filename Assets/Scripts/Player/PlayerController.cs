using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Record;
using Controller;

namespace Player
{
    public class PlayerController : RecordableController
    {
        private float moveSpeed => 玩家配置.main.moveSpeed;
        private float jumpForce => 玩家配置.main.jumpForce;
        private float maxRecordDuration => 玩家配置.main.maxRecordDuration;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f;

        private bool isGrounded;
        private float horizontalInput;
        private bool jumpPressed;

        private void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            if (Input.GetButtonDown("Jump") && isGrounded)
                jumpPressed = true;

            CheckGround();
        }

        private void FixedUpdate()
        {
            var frame = new RecordedFrame { inputX = horizontalInput, jump = jumpPressed };
            ApplyFrame(rb, frame);
            record.RecordFrame(frame);
            jumpPressed = false;
        }

        private void CheckGround()
        {
            if (groundCheck == null) return;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        protected override float GetMoveSpeed() => moveSpeed;
        protected override float GetJumpForce() => jumpForce;
        protected override LayerMask GetGroundLayer() => groundLayer;
        protected override float GetGroundCheckRadius() => groundCheckRadius;

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

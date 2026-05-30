using UnityEngine;
using Record;
using Controller;
using Sirenix.OdinInspector;

namespace Player
{
    public class PlayerController : RecordableController
    {
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f;

        [ShowInInspector, ReadOnly, LabelText("是否在地面")]
        private bool isGrounded;
        private float horizontalInput;
        private bool jumpPressed;
        private bool jumpHeld;
        
        [ShowInInspector, SerializeField]
        private PlayerAnimation avatar;
        
        private void Update()
        {
            base.Update();  // 调用父类 Update（自动停止录制逻辑）

            horizontalInput = Input.GetAxis("Horizontal");
            jumpHeld = Input.GetButton("Jump");
            if (Input.GetButtonDown("Jump") && isGrounded)
                jumpPressed = true;

           avatar.UpdateInput(horizontalInput, jumpPressed);

            CheckGround();
        }

        private RecordedFrame frame => new(){ inputX = horizontalInput, jump = jumpPressed, moveSpeed = 玩家配置.main.moveSpeed , jumpForce = 玩家配置.main.jumpForce };
        private void FixedUpdate()
        {
            record.RecordFrame(ApplyFrame(rb, frame));

            // //松开跳跃键截断上升
            // if (!jumpHeld && rb.velocity.y > 0)
            //     rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            //
            // //下落时加重力
            // if (rb.velocity.y < 0)
            //     rb.velocity += Vector2.up * Physics2D.gravity.y * Time.fixedDeltaTime;

            jumpPressed = false;
        }

        private void CheckGround()
        {
            if (groundCheck == null) return;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }
}

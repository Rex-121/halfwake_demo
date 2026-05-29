using UnityEngine;
using UnityEngine.Events;

namespace Platform
{
    /// <summary>
    /// 带遮罩的横向移动平台：
    /// - 激活时向一个方向移动（可设为单向不往返）
    /// - 取消激活时向相反方向回退，回退完成后触发事件并遮罩
    /// - 可作为其他物体的子物体，激活时重新计算路径
    /// </summary>
    public class MaskedHorizontalPlatform : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private float moveDistance = 3f;
        [SerializeField] private bool moveLeft = true;          // true=向左移动，false=向右
        [SerializeField] private float speed = 2f;              // 前进速度
        [SerializeField] private bool pingPong = false;         // 是否往返（false=单向停在终点）

        [Header("回退设置")]
        [SerializeField] private float reverseSpeed = 2f;      // 回退速度（可等于或不同于前进速度）

        [Header("遮罩")]
        [SerializeField] private Color maskedColor = new Color(1f, 1f, 1f, 0.3f);

        [Header("事件")]
        public UnityEvent OnReverseComplete;   // 回退完成时触发（此时平台已遮罩）

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Collider2D col;

        private Vector3 startPoint;         // 激活时的位置（也是回退的终点）
        private Vector3 endPoint;           // 前进的远端
        private bool movingToEnd;           // 是否朝远端移动
        private bool isActive;              // 激活状态（可见、有碰撞）
        private bool isReversing;           // 是否正在回退

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
            spriteRenderer = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            ApplyMask();
        }

        private void FixedUpdate()
        {
            // 回退逻辑
            if (isReversing)
            {
                // 朝 startPoint 移动（即回退）
                Vector3 newPos = Vector3.MoveTowards(transform.position, startPoint, reverseSpeed * Time.deltaTime);
                rb.MovePosition(newPos);

                if (Vector3.Distance(newPos, startPoint) < 0.01f)
                {
                    // 回退到达，真正停用并遮罩
                    isReversing = false;
                    isActive = false;
                    ApplyMask();
                    OnReverseComplete?.Invoke();
                }
                return;
            }

            // 正常移动
            if (!isActive) return;

            Vector3 target = movingToEnd ? endPoint : startPoint;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            rb.MovePosition(newPosition);

            if (Vector3.Distance(newPosition, target) < 0.01f)
            {
                if (pingPong)
                    movingToEnd = !movingToEnd;   // 往返
                else
                    movingToEnd = false;           // 单向：停在终点，不再反向
            }
        }

        /// <summary>
        /// 激活平台：从当前位置开始向指定方向移动。
        /// 如果正在回退则中断回退，立即进入激活状态。
        /// </summary>
        public void Activate()
        {
            if (isReversing) isReversing = false;  // 中断回退
            if (isActive) return;

            startPoint = transform.position;
            Vector3 dir = moveLeft ? Vector3.left : Vector3.right;
            endPoint = startPoint + dir * moveDistance;
            movingToEnd = true;
            isActive = true;

            if (spriteRenderer) spriteRenderer.color = Color.white;
            if (col) col.enabled = true;
        }

        /// <summary>
        /// 取消激活：开始向相反方向回退（朝起始点移动）。
        /// 回退期间保持可见和碰撞，到达后自动遮罩并触发事件。
        /// </summary>
        public void Deactivate()
        {
            if (!isActive || isReversing) return;
            isReversing = true;
            // isActive 仍为 true，保持可见和碰撞
        }

        private void ApplyMask()
        {
            if (spriteRenderer) spriteRenderer.color = maskedColor;
            if (col) col.enabled = false;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 previewStart = transform.position;
            Vector3 dir = moveLeft ? Vector3.left : Vector3.right;
            Vector3 previewEnd = previewStart + dir * moveDistance;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(previewStart, 0.15f);
            Gizmos.DrawWireSphere(previewEnd, 0.15f);
            Gizmos.DrawLine(previewStart, previewEnd);
        }
    }
}
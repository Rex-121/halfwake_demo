using UnityEngine;
using UnityEngine.Events;

namespace Platform
{
    /// <summary>
    /// 带遮罩的横向移动平台（支持跟随父物体回退）：
    /// - 回退时目标点基于激活时相对父物体的本地位置，父物体移动后仍能正确回退。
    /// - 保留平滑加减速、移动/回退百分比、遮罩和事件。
    /// </summary>
    public class MaskedHorizontalPlatform : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private float moveDistance = 3f;
        [SerializeField] private bool moveLeft = true;
        [SerializeField] private float maxSpeed = 2f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float deceleration = 5f;
        [SerializeField] private bool pingPong = false;
        [Range(0f, 1f)] [SerializeField] private float movePercent = 1f;

        [Header("回退设置")]
        [Range(0f, 1f)] [SerializeField] private float reversePercent = 1f;
        [SerializeField] private float reverseMaxSpeed = 2f;
        [SerializeField] private float reverseAcceleration = 5f;
        [SerializeField] private float reverseDeceleration = 5f;

        [Header("遮罩")]
        [SerializeField] private Color maskedColor = new Color(1f, 1f, 1f, 0.3f);

        [Header("事件")]
        public UnityEvent OnReverseComplete;

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Collider2D col;

        // 路径相关
        private Vector3 startPoint;             // 激活时的世界起点（保留兼容无父物体）
        private Vector3? startLocalPos;         // 激活时相对于父物体的本地坐标（有父物体时使用）
        private Vector3 fullEndPoint;
        private Vector3 targetEndPoint;

        // 状态
        private bool isActive;
        private bool isReversing;
        private float currentSpeed;
        private float currentReverseSpeed;

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
            if (isReversing)
            {
                UpdateReverseMovement();
                return;
            }

            if (!isActive) return;

            MoveTowards(targetEndPoint);
        }

        // ================== 正常移动 ==================
        private void MoveTowards(Vector3 destination)
        {
            Vector3 direction = (destination - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, destination);

            float targetSpeed = 0f;
            if (distanceToTarget > 0.01f)
            {
                float decelDistance = (maxSpeed * maxSpeed) / (2f * deceleration);
                targetSpeed = distanceToTarget > decelDistance ? maxSpeed : maxSpeed * (distanceToTarget / decelDistance);
            }

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 
                (currentSpeed < targetSpeed ? acceleration : deceleration) * Time.deltaTime);

            Vector3 newPos = transform.position + direction * currentSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, destination) <= Vector3.Distance(newPos, destination))
            {
                newPos = destination;
                currentSpeed = 0f;
                if (!pingPong) isActive = false; // 单向到达即停止，但保持实体（遮罩不会激活）
            }

            rb.MovePosition(newPos);
        }

        // ================== 回退移动 ==================
        private void UpdateReverseMovement()
        {
            Vector3 target = GetCurrentReverseTarget();   // 动态目标点
            Vector3 direction = (target - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target);

            float targetSpeed = 0f;
            if (distanceToTarget > 0.01f)
            {
                float decelDistance = (reverseMaxSpeed * reverseMaxSpeed) / (2f * reverseDeceleration);
                targetSpeed = distanceToTarget > decelDistance ? reverseMaxSpeed : reverseMaxSpeed * (distanceToTarget / decelDistance);
            }

            currentReverseSpeed = Mathf.MoveTowards(currentReverseSpeed, targetSpeed,
                (currentReverseSpeed < targetSpeed ? reverseAcceleration : reverseDeceleration) * Time.deltaTime);

            Vector3 newPos = transform.position + direction * currentReverseSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, target) <= Vector3.Distance(newPos, target))
            {
                newPos = target;
                currentReverseSpeed = 0f;
                // 回退完成
                isReversing = false;
                isActive = false;
                ApplyMask();
                OnReverseComplete?.Invoke();
            }

            rb.MovePosition(newPos);
        }

        // 动态计算回退目标（跟随父物体）
        private Vector3 GetCurrentReverseTarget()
        {
            Vector3 effectiveStart = startPoint;
            if (startLocalPos.HasValue && transform.parent != null)
                effectiveStart = transform.parent.TransformPoint(startLocalPos.Value);
            return Vector3.Lerp(transform.position, effectiveStart, reversePercent);
        }

        // ================== 公共接口 ==================
        public void Activate()
        {
            if (isReversing) isReversing = false;
            if (isActive) return;

            startPoint = transform.position;
            // 记录相对于父物体的本地位置（若存在）
            if (transform.parent != null)
                startLocalPos = transform.parent.InverseTransformPoint(startPoint);
            else
                startLocalPos = null;

            Vector3 dir = moveLeft ? Vector3.left : Vector3.right;
            fullEndPoint = startPoint + dir * moveDistance;
            targetEndPoint = Vector3.Lerp(startPoint, fullEndPoint, movePercent);

            currentSpeed = 0f;
            isActive = true;

            if (spriteRenderer) spriteRenderer.color = Color.white;
            if (col) col.enabled = true;
        }

        public void Deactivate()
        {
            if (!isActive || isReversing) return;

            // 回退百分比为 0 则直接完成（停在当前位置并遮罩）
            if (reversePercent <= 0.001f)
            {
                isActive = false;
                ApplyMask();
                OnReverseComplete?.Invoke();
                return;
            }

            isReversing = true;
            currentReverseSpeed = 0f;
            // isActive 保持 true 以确保实体可见和碰撞，直到回退结束
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
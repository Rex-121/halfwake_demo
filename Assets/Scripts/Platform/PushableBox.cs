using System.Collections;
using UnityEngine;

namespace Platform
{
    /// <summary>
    /// 可推动的箱子：接收来自角色（本体或分身）的推动指令，
    /// 沿指定方向移动固定距离，碰到障碍物则无法推动。
    /// </summary>
    public class PushableBox : MonoBehaviour
    {
        [Header("推动设置")]
        [SerializeField] private float pushDistance = 1f;       // 每次推动移动的距离（通常为一个格子大小）
        [SerializeField] private float pushDuration = 0.2f;     // 推动动画的持续时间
        [SerializeField] private LayerMask obstacleMask;         // 障碍物所在的图层（墙壁、其他箱子等）

        private bool isMoving = false;   // 是否正在移动中，防止重复推动

        /// <summary>
        /// 由角色（本体或分身回放时）调用的推动方法。
        /// </summary>
        /// <param name="direction">推动方向（必须归一化）</param>
        public void Push(Vector2 direction)
        {
            // 正在移动或方向无效则忽略
            if (isMoving || direction == Vector2.zero) return;

            // 射线检测前方是否有障碍物
            Vector2 origin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, pushDistance, obstacleMask);
            if (hit.collider == null)  // 前方无障碍，可以推动
            {
                StartCoroutine(MoveTo(origin + direction * pushDistance));
            }
        }

        /// <summary>
        /// 平滑移动箱子到目标位置。
        /// </summary>
        private IEnumerator MoveTo(Vector2 target)
        {
            isMoving = true;
            Vector2 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < pushDuration)
            {
                transform.position = Vector2.Lerp(startPos, target, elapsed / pushDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = target;
            isMoving = false;
        }

        /// <summary>
        /// 可选的自动推动支持：当角色紧贴箱子并按下方向键时自动推动。
        /// 如果角色程序员已经通过脚本调用 Push()，可移除此方法。
        /// </summary>
        private void OnCollisionStay2D(Collision2D collision)
        {
            // 只处理带有有效 Tag 的角色碰撞（避免误触）
            if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("Clone"))
                return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 dir = Vector2.zero;
            if (Mathf.Abs(h) > 0.1f) dir = new Vector2(h, 0);
            else if (Mathf.Abs(v) > 0.1f) dir = new Vector2(0, v);

            if (dir != Vector2.zero)
            {
                Push(dir.normalized);
            }
        }
    }
}
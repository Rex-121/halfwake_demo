using UnityEngine;

namespace Platform
{
    /// <summary>
    /// 出口门：只有本体（Tag 为 Player）触碰时触发通关，
    /// 分身（Clone）触碰无效，符合设计规则。
    /// </summary>
    public class ExitDoor : MonoBehaviour
    {
        [Tooltip("本体的 Tag，通常为 \"Player\"")]
        [SerializeField] private string playerTag = "Player";

        /// <summary>
        /// 当物体进入触发器时调用。
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 仅当碰撞体的 Tag 与 playerTag 匹配时才视为本体通关
            if (other.CompareTag(playerTag))
            {
                // 这里调用关卡完成逻辑，例如加载下一关
                Debug.Log("关卡完成！");
                // GameManager.Instance.CompleteLevel();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platform
{
    /// <summary>
    /// 门：监听多个压力板的状态，当所有要求的压力板都被激活时开门，
    /// 任意一个失效就关门。
    /// </summary>
    public class Door : MonoBehaviour
    {
        [Tooltip("需要全部激活才能打开的压力板列表")]
        [SerializeField] private PressurePlate[] requiredPlates;

        // 记录每个压力板当前的激活状态
        private Dictionary<PressurePlate, bool> plateStates = new Dictionary<PressurePlate, bool>();
        // 当前门是否已打开
        private bool isOpen = false;

        private void Start()
        {
            // 初始化状态字典并订阅每个压力板的状态变化事件
            foreach (var plate in requiredPlates)
            {
                if (plate == null) continue;
                plateStates[plate] = false;
                plate.OnActivationChanged += OnPlateStateChanged;
            }
            UpdateDoorState();
        }

        private void OnDestroy()
        {
            // 取消订阅，避免内存泄漏
            foreach (var plate in requiredPlates)
            {
                if (plate != null)
                    plate.OnActivationChanged -= OnPlateStateChanged;
            }
        }

        /// <summary>
        /// 当某个压力板激活状态改变时回调，更新记录并检查门的状态。
        /// </summary>
        private void OnPlateStateChanged(PressurePlate plate, bool activated)
        {
            plateStates[plate] = activated;
            UpdateDoorState();
        }

        /// <summary>
        /// 遍历所有压力板，如果全部激活则开门，否则关门。
        /// </summary>
        private void UpdateDoorState()
        {
            bool allActivated = true;
            foreach (bool state in plateStates.Values)
            {
                if (!state)
                {
                    allActivated = false;
                    break;
                }
            }

            if (allActivated != isOpen)
            {
                isOpen = allActivated;
                if (isOpen)
                    OpenDoor();
                else
                    CloseDoor();
            }
        }

        /// <summary>
        /// 开门的具体实现：禁用碰撞体（角色可通过），并改变颜色提示。
        /// 可根据需要替换为播放动画等。
        /// </summary>
        private void OpenDoor()
        {
            if (TryGetComponent<Collider2D>(out var col))
                col.enabled = false;
            if (TryGetComponent<SpriteRenderer>(out var sr))
                sr.color = Color.green;
        }

        /// <summary>
        /// 关门的具体实现：启用碰撞体，恢复颜色。
        /// </summary>
        private void CloseDoor()
        {
            if (TryGetComponent<Collider2D>(out var col))
                col.enabled = true;
            if (TryGetComponent<SpriteRenderer>(out var sr))
                sr.color = Color.red;
        }
    }
}

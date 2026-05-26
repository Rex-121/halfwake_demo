using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platform
{
    /// <summary>
    /// 压力板：当任意有效角色（本体或分身）站在上面时激活，
    /// 离开时取消激活。通过事件通知关联的门。
    /// </summary>
    public class PressurePlate : MonoBehaviour
    {
        /// <summary>
        /// 激活状态改变时触发的事件，参数为（压力板自身, 是否激活）。
        /// 门等机关监听此事件来更新开关状态。
        /// </summary>
        public event Action<PressurePlate, bool> OnActivationChanged;

        [Tooltip("可以触发压力板的 Tag 列表，例如本体为 Player，分身为 Clone")]
        [SerializeField] private string[] activatorTags = { "Player", "Clone" };

        // 当前站在板上的所有碰撞体（用于统计数量，防止重复触发）
        private HashSet<Collider2D> collidersInside = new HashSet<Collider2D>();
        // 上一帧的激活状态，用于避免重复触发事件
        private bool wasActivated = false;

        /// <summary>
        /// 有物体进入触发器范围时调用。
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsValidActivator(other))
            {
                collidersInside.Add(other);
                CheckActivation();
            }
        }

        /// <summary>
        /// 有物体离开触发器范围时调用。
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (collidersInside.Remove(other))
            {
                CheckActivation();
            }
        }

        /// <summary>
        /// 检查碰撞体是否属于有效的触发者（通过 Tag 判断）。
        /// </summary>
        private bool IsValidActivator(Collider2D col)
        {
            foreach (string tag in activatorTags)
            {
                if (col.CompareTag(tag))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 更新激活状态，并在发生变化时触发事件。
        /// </summary>
        private void CheckActivation()
        {
            bool isActivated = collidersInside.Count > 0;
            if (isActivated != wasActivated)
            {
                wasActivated = isActivated;
                // 通知所有监听者（如门）状态已改变
                OnActivationChanged?.Invoke(this, isActivated);
            }
        }

        /// <summary>
        /// 当压力板自身被禁用或销毁时调用，
        /// 清理所有碰撞体记录并强制重置激活状态。
        /// </summary>
        private void OnDisable()
        {
            collidersInside.Clear();
            if (wasActivated)
            {
                wasActivated = false;
                OnActivationChanged?.Invoke(this, false);
            }
        }
    }
}
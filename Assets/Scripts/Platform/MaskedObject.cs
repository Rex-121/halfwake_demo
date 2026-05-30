using UnityEngine;

namespace Platform
{
    /// <summary>
    /// 通用遮罩：控制物体的可见性（半透明）和碰撞启用/禁用。
    /// 挂载到带 SpriteRenderer 和 Collider2D 的对象上。
    /// </summary>
    public class MaskedObject : MonoBehaviour
    {
        [Header("遮罩状态")]
        [SerializeField] private Color maskedColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private bool startMasked = true;   // 初始是否遮罩

        private SpriteRenderer spriteRenderer;
        private Collider2D col;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();

            if (startMasked)
                ApplyMask();
            else
                Reveal();
        }

        /// <summary> 进入遮罩状态：半透明 + 无碰撞 </summary>
        public void ApplyMask()
        {
            if (spriteRenderer) spriteRenderer.color = maskedColor;
            if (col) col.enabled = false;
        }

        /// <summary> 取消遮罩：完全可见 + 开启碰撞 </summary>
        public void Reveal()
        {
            if (spriteRenderer) spriteRenderer.color = Color.white;
            if (col) col.enabled = true;
        }
    }
}
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Sprite[] jumpSprites;

        public Sprite[] JumpSprites => jumpSprites;
        [SerializeField] private float jumpFrameInterval = 0.15f;
        [SerializeField] private float landFrameInterval = 0.1f;

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D parentRb;
        private bool wasInAir;

        [ShowInInspector, SerializeField]
        private Transform avatar;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            parentRb = GetComponentInParent<Rigidbody2D>();
        }

        public void UpdateInput(float horizontal, bool jump)
        {
            if (horizontal != 0 && avatar != null)
                avatar.localScale = new Vector3(-Mathf.Sign(horizontal), 1, 1);
        }

        private void Update()
        {
            if (parentRb == null) return;

            bool inAir = Mathf.Abs(parentRb.velocity.y) > 0.1f;

            if (inAir)
            {
                animator.enabled = false;
                PlayAirFrames();
            }
            else if (wasInAir)
            {
                animator.enabled = false;
                PlayLandFrames();
            }
            else
            {
                animator.enabled = true;
                animator.SetBool("walking", Mathf.Abs(parentRb.velocity.x) > 0.1f);
            }

            wasInAir = inAir;
        }

        private float airTimer;
        private void PlayAirFrames()
        {
            airTimer += Time.deltaTime;
            int frame = Mathf.Min((int)(airTimer / jumpFrameInterval), 2);
            if (jumpSprites != null && frame < jumpSprites.Length)
                spriteRenderer.sprite = jumpSprites[frame];
        }

        private float landTimer;
        private void PlayLandFrames()
        {
            landTimer += Time.deltaTime;
            int frame = Mathf.Min((int)(landTimer / landFrameInterval) + 3, jumpSprites.Length - 1);
            if (jumpSprites != null && frame < jumpSprites.Length)
                spriteRenderer.sprite = jumpSprites[frame];

            if (landTimer >= landFrameInterval * 2)
            {
                landTimer = 0;
                airTimer = 0;
                wasInAir = false;
            }
        }
    }
}

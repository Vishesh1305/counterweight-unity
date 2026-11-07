using UnityEngine;

// Shared run and jump for both players. Scout and Heavy differ only in the serialized stats.
namespace Counterweight.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private GroundCheck groundCheck;

        [Header("Role Stats")]
        [SerializeField, Min(0f)] private float moveSpeed = 7f;
        [Tooltip("Peak jump height in world units. 1 unit = 1 tile.")]
        [SerializeField, Min(0f)] private float jumpHeight = 3f;

        [Header("Jump Feel")]
        [Tooltip("Seconds after walking off a ledge during which a jump still counts.")]
        [SerializeField, Min(0f)] private float coyoteTime = 0.1f;
        [Tooltip("Seconds a jump press is remembered before landing.")]
        [SerializeField, Min(0f)] private float jumpBufferTime = 0.1f;
        [Tooltip("Upward speed is multiplied by this when jump is released early.")]
        [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;

        private Rigidbody2D _body;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private bool _isRising;

        public bool IsGrounded { get; private set; }
        public Vector2 Velocity => _body.linearVelocity;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();

            if (input == null || groundCheck == null)
            {
                Debug.LogError($"{name}: PlayerMovement needs Input and Ground Check assigned.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (input != null)
                input.JumpPressed += BufferJump;
        }

        private void OnDisable()
        {
            if (input != null)
                input.JumpPressed -= BufferJump;
        }

        private void Update()
        {
            if (_jumpBufferTimer > 0f)
                _jumpBufferTimer -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            groundCheck.Refresh();
            Vector2 velocity = _body.linearVelocity;

            // Right after takeoff the feet box can still touch the ground for one step. Ignoring it blocks a second jump.
            IsGrounded = groundCheck.IsGrounded && !(_isRising && velocity.y > 0f);

            _coyoteTimer = IsGrounded ? coyoteTime : _coyoteTimer - Time.fixedDeltaTime;

            velocity.x = input.MoveInput * moveSpeed;
            velocity.x += GetCarryVelocityX(); 

            if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
            {
                velocity.y = CalculateJumpVelocity();
                _jumpBufferTimer = 0f;
                _coyoteTimer = 0f; // Spent, so coyote time can never give a second jump.
                _isRising = true;
            }
            else if (_isRising && (velocity.y <= 0f || !input.JumpHeld))
            {
                if (velocity.y > 0f)
                    velocity.y *= jumpCutMultiplier;

                _isRising = false;
            }

            _body.linearVelocity = velocity;
        }

        private void BufferJump()
        {
            _jumpBufferTimer = jumpBufferTime;
        }

        private float CalculateJumpVelocity()
        {
            // v = sqrt(2gh): the launch speed that peaks at exactly jumpHeight.
            float gravity = Mathf.Abs(Physics2D.gravity.y * _body.gravityScale);
            return Mathf.Sqrt(2f * gravity * jumpHeight);
        }

        // Horizontal speed of whatever we stand on. Players use zero friction, so without this they would slide off movers. 
        private float GetCarryVelocityX() 
        { 
            if (!IsGrounded) 
                return 0f; 

            Rigidbody2D ground = groundCheck.GroundBody; 
            return ground != null ? ground.linearVelocity.x : 0f; 
        } 
    }
}
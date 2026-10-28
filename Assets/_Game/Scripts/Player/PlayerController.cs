using UnityEngine;
using UnityEngine.InputSystem;

namespace Eclipse.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerData playerData;

        public PlayerStats Stats { get; private set; }

        [SerializeField]
        private InputActionReference moveAction;

        private Rigidbody2D body;
        private Vector2 moveInput;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();

            if (playerData == null)
            {
                Debug.LogError(
                    "PlayerController requires PlayerData.",
                    this);

                enabled = false;
                return;
            }

            Stats = new PlayerStats(playerData);
        }

        private void OnEnable()
        {
            if (moveAction == null)
            {
                Debug.LogError(
                    "PlayerController requires a Move InputActionReference.",
                    this);

                enabled = false;
                return;
            }

            moveAction.action.Enable();
        }

        private void OnDisable()
        {
            if (moveAction != null)
            {
                moveAction.action.Disable();
            }
        }

        private void Update()
        {
            moveInput = moveAction.action.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            Vector2 movementInput =
                Vector2.ClampMagnitude(moveInput, 1f);

            Vector2 nextPosition =
                body.position +
                movementInput * Stats.MoveSpeed * Time.fixedDeltaTime;

            body.MovePosition(nextPosition);
        }
    }
}
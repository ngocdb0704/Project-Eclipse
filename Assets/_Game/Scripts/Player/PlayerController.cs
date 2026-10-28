using UnityEngine;
using UnityEngine.InputSystem;

namespace Eclipse.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float moveSpeed = 5f;

        [SerializeField]
        private InputActionReference moveAction;

        private Rigidbody2D body;
        private Vector2 moveInput;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
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
                movementInput * moveSpeed * Time.fixedDeltaTime;

            body.MovePosition(nextPosition);
        }
    }
}
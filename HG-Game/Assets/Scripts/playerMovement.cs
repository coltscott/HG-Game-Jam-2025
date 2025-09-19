using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
  [SerializeField] private InputActionAsset InputActions;

  private InputAction m_moveAction;
  private InputAction m_jumpAction;

  [SerializeField] private Rigidbody2D body;
  [SerializeField] private Transform groundCheck;
  private float groundCheckRadius = 0.2f;
  [SerializeField] private LayerMask groundLayer;

  private Vector2 moveInput;
  private float speed = 8f;
  private float jumpingPower = 16f;

  private void Awake()
  {
    m_moveAction = InputSystem.actions.FindAction("Move");
    m_jumpAction = InputSystem.actions.FindAction("Jump");
    body = GetComponent<Rigidbody2D>();
  }

  private void Update()
  {
    moveInput = m_moveAction.ReadValue<Vector2>();

    if (m_jumpAction.WasPressedThisFrame() && isGrounded())
    {
      Jump();
    }
  }

  private void Jump()
  {
    body.AddForceAtPosition(new Vector2(0, jumpingPower), Vector2.up, ForceMode2D.Impulse);
  }

  private void FixedUpdate()
  {
    body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
  }

  private bool isGrounded()
  {
    return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    // return false;
  }
}

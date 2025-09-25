

using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
  [SerializeField] private InputActionAsset InputActions;

  private InputAction m_moveAction;
  private InputAction m_jumpAction;
  private InputAction m_dashAction;

  [SerializeField] private Rigidbody2D body;
  [SerializeField] private Transform groundCheck;
  private float groundCheckRadius = 0.04f;
  [SerializeField] private LayerMask groundLayer;

  private Vector2 moveInput;
  private float speed = 4f;
  private float jumpingPower = 9f;
  private float defaultGravity = -9.81f;
  private float scaleGravity = 4;
  private bool dashAvailable = true;
  private float dashTime = 0.15f;
  private bool isDashing = false;
  private float dashPower = 12f;
  private float dashEnd = 0f;
  private float dashDirection = 0;
  private bool doubleJumpReset = true;

  private void Awake()
  {
    m_moveAction = InputSystem.actions.FindAction("Move");
    m_jumpAction = InputSystem.actions.FindAction("Jump");
    m_dashAction = InputSystem.actions.FindAction("Dash");
    body = GetComponent<Rigidbody2D>();
  }

  private void Update()
  {
    moveInput = m_moveAction.ReadValue<Vector2>();
    // Debug.Log(moveInput);

    if (m_jumpAction.WasPressedThisFrame() && (isGrounded() || doubleJumpReset))
    {
      Jump();
    }

    if (m_dashAction.WasPressedThisFrame() && dashAvailable && moveInput.x != 0f)
    {
      Dash();
    }
  }

  private void Jump()
  {
    if (!isGrounded())
    {
      doubleJumpReset = false;
    }
    body.linearVelocity = new Vector2(body.linearVelocity.x, 0);
    body.AddForceAtPosition(new Vector2(0, jumpingPower), Vector2.up, ForceMode2D.Impulse);
  }

  private void Dash()
  {
    Debug.Log("dash start");
    dashAvailable = false;
    isDashing = true;
    dashDirection = moveInput.x;
    dashEnd = Time.time + dashTime;
  }

  // private void OnCollisionEnter2D(Collision2D collision)
  // {
  //   switch (collision.gameObject.layer)
  //   {
  //     default:
  //       break;
  //   }
  // }

  private void OnCollisionStay2D(Collision2D collision)
  {
    switch (collision.gameObject.layer)
    {
      case 8:
        dashAvailable = true;
        doubleJumpReset = true;
        // Debug.Log("ground layer");
        break;
      default:
        break;
    }
    // Debug.Log("Collision stay with" + collision.gameObject.name);
  }

  // private void OnCollisionExit2D(Collision2D collision)
  // {
  //   Debug.Log("Collision exit with" + collision.gameObject.name);
  // }

  private void FixedUpdate()
  {
    if (isDashing)
    {
      if (Time.time >= dashEnd)
      {
        isDashing = false;
      }
      // else
      // {
        
      // }
    }

    if (isDashing)
    {
      Debug.Log("test");
      body.linearVelocity = new Vector2(dashDirection * dashPower, 0);
    }
    else
    {
      body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
      body.linearVelocity += new Vector2(0, defaultGravity * scaleGravity * Time.fixedDeltaTime);
    }
    
  }

  private bool isGrounded()
  {
    return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    // return false;
  }
}

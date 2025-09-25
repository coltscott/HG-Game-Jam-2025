

using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class playerMovement : MonoBehaviour
{
  // Input actions
  private InputActionAsset InputActions;
  private InputAction m_moveAction;
  private InputAction m_jumpAction;
  private InputAction m_dashAction;

  // Parts of the player
  private Rigidbody2D body;
  [SerializeField] private Transform groundCheck;
  [SerializeField] private Transform leftCheck;
  [SerializeField] private Transform rightCheck;
  // Layers
  [SerializeField] private LayerMask groundLayer;
  [SerializeField] private LayerMask wallLayer;

  // Ground check parameters
  private float groundCheckRadius = 0.04f;

  // Basic movement storage and parameters
  private Vector2 moveInput;
  private float speed = 4f;

  //Gravity
  private float defaultGravity = -9.81f;
  private float scaleGravity = 4f;

  // Jump
  [SerializeField] private int jumpsAvailiable = 2;

  private int maxJumps = 2;
  private float jumpingPower = 16f;

  // Dash
  private bool dashAvailable = true;
  private float dashTime = 0.15f;
  private bool isDashing = false;
  private float dashPower = 12f;
  private float dashEnd = 0f;
  private float dashDirection = 0;

  // Wall mechanics
  [SerializeField] private float wallStickEnd = 0.0f;
  private float wallStickDuration = 2f;
  [SerializeField] private bool isOnWall = false;
  [SerializeField] private bool wallSticky = false;
  private float scaleStickyGravity = 2f;
  private float wallDirection = 0;

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

    if (m_jumpAction.WasPressedThisFrame() && (isGrounded() || jumpsAvailiable > 0))
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
    jumpsAvailiable -= 1;
    if (isOnWall && (wallDirection != moveInput.x))
    {
      body.linearVelocity = new Vector2(body.linearVelocity.x, 0);
    }
    else
    {
      body.linearVelocity = new Vector2(body.linearVelocity.x, 0);
    }
    
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

  private void OnCollisionEnter2D(Collision2D collision)
  {
    switch (collision.gameObject.layer)
    {
      case 8:
        jumpsAvailiable = 2;
        break;
      case 9:
        wallDirection = wallCollisionDirection();
        isOnWall = true;
        wallStickEnd = Time.time + wallStickDuration;
        dashAvailable = true;
        jumpsAvailiable = maxJumps;
        wallSticky = true;
        break;
      default:
        break;
    }
  }

  private void OnCollisionStay2D(Collision2D collision)
  {
    switch (collision.gameObject.layer)
    {
      case 8:
        dashAvailable = true;
        // Debug.Log("ground layer");
        break;
      case 9:
        if (wallStickEnd < Time.time)
        {
          wallSticky = false;
        }
        // Debug.Log("walled");

        break;
      default:
        break;
    }
    // Debug.Log("Collision stay with" + collision.gameObject.name);
  }

  private void OnCollisionExit2D(Collision2D collision)
  {
    switch (collision.gameObject.layer)
    {
      case 8:
        if (jumpsAvailiable == maxJumps && isGrounded())
        {
          jumpsAvailiable -= 1;
        }

        break;
      case 9:
        isOnWall = false;
        goto case 8;
        // break;
      default:
        break;
    }
    // Debug.Log("Collision exit with" + collision.gameObject.name);
  }

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
      body.linearVelocity = new Vector2(dashDirection * dashPower, 0);
    }
    else
    {
      if (wallSticky && isOnWall)
      {
        body.linearVelocity = new Vector2(moveInput.x * speed, 0);
        body.linearVelocity += new Vector2(0, 0);
      }
      else if (isOnWall && !wallSticky)
      {
        body.linearVelocity = new Vector2(0, body.linearVelocity.y);
        if (moveInput.x != wallDirection)
        {
          body.linearVelocity += new Vector2(moveInput.x * speed,  defaultGravity * scaleGravity * Time.fixedDeltaTime);
        }
        else
        {
          body.linearVelocity += new Vector2(0,  defaultGravity * scaleStickyGravity * Time.fixedDeltaTime);
        }
        
      }
      else
      {
        body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
        body.linearVelocity += new Vector2(0, defaultGravity * scaleGravity * Time.fixedDeltaTime);
      }

    }
    // Debug.Log(body.linearVelocity);

  }

  private bool isGrounded()
  {
    return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    // return false;
  }

  private float wallCollisionDirection()
  {
    if (Physics2D.OverlapCircle(leftCheck.position, groundCheckRadius, wallLayer))
    {
      Debug.Log("left");
      return -1f;
    }
    if (Physics2D.OverlapCircle(rightCheck.position, groundCheckRadius, wallLayer))
    {
      Debug.Log("right");
      return 1f;
    }

    return 0f;
  }
}



using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum PlayerState
{
  Stable = 1,
  Dashing = 2,
  Jumping = 3
}

public enum LayerNumber : int
{
  Ground = 8,
  Wall = 9
}

public class playerMovement : MonoBehaviour
{
  // Player state
  PlayerState playerState = PlayerState.Stable;

  // Input actions
  // private InputActionAsset InputActions;
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
  private float jumpingPower = 12f;
  private float jumpTime = 0.15f;
  private float jumpEnd;
  private float jumpDirection = 0;
  private bool triedFlipped = false;

  // Dash
  private bool dashAvailable = true;
  private float dashTime = 0.15f;
  private float dashPower = 12f;
  private float dashEnd = 0f;
  private float dashDirection = 0;

  // Wall mechanics
  [SerializeField] private float wallStickEnd = 0.0f;
  private float wallStickDuration = 2f;
  [SerializeField] private bool isOnWall = false;
  [SerializeField] private bool wallSticky = false;
  private float scaleStickyGravity = 0.5f;
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

    if (m_jumpAction.WasPressedThisFrame() && (isGrounded() || jumpsAvailiable > 0) && (playerState == PlayerState.Stable))
    {
      Jump();
    }

    if (m_dashAction.WasPressedThisFrame() && dashAvailable && (moveInput.x != 0f) && (playerState == PlayerState.Stable))
    {
      Dash();
    }
  }

  private void Jump()
  {
    Debug.Log("start jump");
    jumpsAvailiable -= 1;
    playerState = PlayerState.Jumping;
    jumpEnd = Time.time + jumpTime;
    jumpDirection = moveInput.x;
    triedFlipped = false;


    // Move to FixedUpdate()
  }

  private void Dash()
  {
    dashAvailable = false;
    playerState = PlayerState.Dashing;
    dashDirection = moveInput.x;
    dashEnd = Time.time + dashTime;
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    switch (collision.gameObject.layer)
    {
      case (int) LayerNumber.Ground:
        jumpsAvailiable = 2;
        break;
      case (int) LayerNumber.Wall:
        playerState = PlayerState.Stable;
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
      case (int) LayerNumber.Ground:
        dashAvailable = true;
        break;
      case (int) LayerNumber.Wall:
        if (wallSticky && wallStickEnd < Time.time)
        {
          wallSticky = false;
        }

        break;
      default:
        break;
    }
  }

  private void OnCollisionExit2D(Collision2D collision)
  {
    switch (collision.gameObject.layer)
    {
      case (int) LayerNumber.Ground:
        if (jumpsAvailiable == maxJumps && isGrounded())
        {
          jumpsAvailiable -= 1;
        }

        break;
      case (int) LayerNumber.Wall:
        isOnWall = false;
        goto case 8;
      // break;
      default:
        break;
    }
  }

  private void FixedUpdate()
  {
    switch (playerState)
    {
      case PlayerState.Stable:
        if (moveInput.x != wallDirection)
        {
          wallSticky = false;
        }
        if (wallSticky && isOnWall)
        {
          body.linearVelocity = new Vector2(moveInput.x * speed, 0);
        }
        else if (!wallSticky && isOnWall)
        {
          body.linearVelocity = new Vector2(0, body.linearVelocity.y);
          if (moveInput.x != wallDirection)
          {
            body.linearVelocity += new Vector2(moveInput.x * speed, defaultGravity * scaleGravity * Time.fixedDeltaTime);
          }
          else
          {
            body.linearVelocity += new Vector2(0, defaultGravity * scaleStickyGravity * Time.fixedDeltaTime);
          }
        }
        else
        {
          body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
          body.linearVelocity += new Vector2(0, defaultGravity * scaleGravity * Time.fixedDeltaTime);
        }
        break;
      case PlayerState.Dashing:
        if (Time.time >= dashEnd)
        {
          playerState = PlayerState.Stable;
          goto case PlayerState.Stable;
        }

        body.linearVelocity = new Vector2(dashDirection * dashPower, 0);
        break;
      case PlayerState.Jumping:
        if (Time.time >= jumpEnd)
        {
          playerState = PlayerState.Stable;
          jumpDirection = 0;
          goto case PlayerState.Stable;
        }
        if (isOnWall && (wallDirection == moveInput.x) && !triedFlipped)
        {
          Debug.Log("flip");
          jumpDirection = -jumpDirection;
        }
        triedFlipped = true;
        body.linearVelocity = new Vector2(jumpDirection * speed, 0);

        body.AddForceAtPosition(new Vector2(0, jumpingPower * (jumpEnd - Time.time + 0.5f)), Vector2.up, ForceMode2D.Impulse);

        Debug.Log(body.linearVelocity);


        break;
    }

  }

  private bool isGrounded()
  {
    return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
  }

  private float wallCollisionDirection()
  {
    if (Physics2D.OverlapCircle(leftCheck.position, groundCheckRadius, wallLayer))
    {
      return -1f;
    }
    if (Physics2D.OverlapCircle(rightCheck.position, groundCheckRadius, wallLayer))
    {
      return 1f;
    }

    return 0f;
  }
}

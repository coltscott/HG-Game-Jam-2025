using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour {
  	[SerializeField] private InputActionAsset InputActions;

 	private InputAction m_moveAction;
  	private InputAction m_jumpAction;

  	[SerializeField] private Rigidbody2D body;
  	[SerializeField] private Transform groundCheck;
  	private float groundCheckRadius = 0.04f;
  	[SerializeField] private LayerMask groundLayer;

  	private Vector2 moveInput;
  	private float speed = 8f;
  	private float jumpingPower = 12f;

    private float defaultGravity = -9.81f;
    private float scaleGravity = 4;

    public bool jump = false;

  	private void Awake()
  {
    m_moveAction = InputSystem.actions.FindAction("Move");
    m_jumpAction = InputSystem.actions.FindAction("Jump");
    body = GetComponent<Rigidbody2D>();
  }

	  private void Update() {
    	  moveInput = m_moveAction.ReadValue<Vector2>();

    	  if (m_jumpAction.WasPressedThisFrame() && isGrounded()) {
            Debug.Log("Jump");
      		  Jump();
    	  }
  	}

  	private void Jump() {
    	  body.AddForceAtPosition(new Vector2(0, jumpingPower), Vector2.up, ForceMode2D.Impulse);
  	}

  private void FixedUpdate() {
    if (Mathf.Abs(moveInput.x) < 0.01)
    {
      moveInput.x = 0;
    }
    body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);

    body.linearVelocity += new Vector2(0, defaultGravity * scaleGravity * Time.fixedDeltaTime);
    Debug.Log(body.linearVelocity);
  	}

  	private bool isGrounded() {
      jump = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
      return jump;
  	}
// Debug.Log("Collided with: " + collision.gameObject.name);
}

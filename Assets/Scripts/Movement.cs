using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public static Movement Instance { get; private set; }
    private Rigidbody2D rb;
    private DistanceJoint2D dj;
    private SpriteRenderer sr;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    private float horizontal;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 5f;
    private bool canJump = true;
    private bool canCancelJump = false;

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 1f;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckArea = new Vector2(0.9f, 0.05f);
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [SerializeField] private Grappler grappler;
    private Vector2 lastGoodPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dj = GetComponent<DistanceJoint2D>();
        sr = GetComponent<SpriteRenderer>();
        grappler = GetComponent<Grappler>();
    }

    // Update is called once per frame
    void Update()
    {
        canJump = GroundCheck();
        ProcessMovement();
        ProcessLastGoodPos();
        ProcessGravity();
    }

    // Reads up/down/left/right input
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();

        horizontal = movement.x;

        if(horizontal != 0f)
            sr.flipY = horizontal < 0;
    }

    // Processes the movement of the player
    private void ProcessMovement()
    {
        Vector2 velocity = transform.right * horizontal * moveSpeed;
        if(!isGrounded && (Math.Abs(rb.linearVelocityX) > Math.Abs(velocity.x)))
        {
            // if(transform.position.y > dj.connectedAnchor.y)
            // {
            //     rb.linearVelocity = Vector2.zero;
            // }

            // if(velocity.x == 0)
            // {
            //     rb.linearVelocityX -= rb.linearVelocityX * Time.deltaTime/5;
            // }
            // else
            // {
            //     if(velocity.x < 0)
            //     {
            //         rb.linearVelocityX = Mathf.Max(-15f, rb.linearVelocityX + velocity.x / 5);
            //     }
            //     else if (velocity.x > 0)
            //     {
            //         rb.linearVelocityX = Mathf.Min(15f, rb.linearVelocityX + velocity.x / 5);
            //     }
            // }

            if(grappler.isGrappling)
                rb.AddForceX(velocity.x);
        }
        else
        {
            rb.linearVelocityX = velocity.x;
        }

        rb.linearVelocityY = Mathf.Min(rb.linearVelocityY, 15);

        // Debug.Log("velocity: " + rb.linearVelocity);
    }

    // Reads jump input
    public void Jump(InputAction.CallbackContext context)
    {
        if(canJump && context.performed)
        {   
            // Debug.Log("jump performed");
            rb.linearVelocityY = jumpPower;
            canJump = false;
            canCancelJump = true;
        }
        else if(canCancelJump && context.canceled)
        {
            // Debug.Log("jump canceled");
            rb.linearVelocityY *= 0.5f;
            canJump = false;
            canCancelJump = false;
        }
    }

    // Processes gravity on the players
    public void ProcessGravity()
    {
        if(isGrounded)
            rb.gravityScale = 0.0f;
        else
            rb.gravityScale = baseGravity;
    }

    // Is the player on the ground?
    public bool GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckArea, 0, groundLayer))
        {
            // Debug.Log("isGrounded == true");
            return isGrounded = true;
        }
        // Debug.Log("isGrounded == false");
        return isGrounded = false;
    }

    public void ProcessLastGoodPos()
    {
        if(Physics2D.OverlapCapsule(transform.position, new Vector2(1.0f, 2.0f), CapsuleDirection2D.Vertical, 0.0f, groundLayer))
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = lastGoodPos;
        }
        else
        {
            lastGoodPos = transform.position;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draws GroundCheck in green.
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckArea);
    }
}

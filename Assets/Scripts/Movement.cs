using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        ProcessMovement();
        ProcessGravity();
    }

    // Reads up/down/left/right input
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();

        horizontal = movement.x;
    }

    // Processes the movement of the player
    private void ProcessMovement()
    {
        Vector2 velocity = transform.right * horizontal * moveSpeed;

        rb.linearVelocityX = velocity.x;
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
            canJump = true;
            return isGrounded = true;
        }
        // Debug.Log("isGrounded == false");
        return isGrounded = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draws GroundCheck in green.
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckArea);
    }
}

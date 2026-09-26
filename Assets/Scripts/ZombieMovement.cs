using UnityEngine;
using UnityEngine.InputSystem;

public class ZombieMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 10f;
    private bool canJump = true;

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2f;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckArea = new Vector2(0.9f, 0.05f);
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    public float Destination;
    public float DestTimer;
    public bool SeeSpider;
    public Transform Eye;
    public LayerMask Obstacles;

    public int LeftEdge;
    public int RightEdge;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destination = Random.Range(LeftEdge, RightEdge);
    }

    // Update is called once per frame
    void Update()
    {
        if(!SeeSpider)
        {
            DestTimer += Time.deltaTime;
            if(DestTimer > 5 || Mathf.Abs(Destination - transform.position.x) < 2)
            {
                DestTimer = 0;
                Destination = Random.Range(LeftEdge, RightEdge);
            }
        }
        GroundCheck();
        ProcessMovement();
        ProcessGravity();
    }

    // Processes the movement of the player
    private void ProcessMovement()
    {
        Vector2 velocity = transform.right * moveSpeed;
        velocity.y = rb.linearVelocityY;
        if(velocity.y < -50)
        {
            velocity.y = -50;
        }
        if(Destination - transform.position.x < 0)
        {
            //Debug.DrawRay((Eye.position - 1.45f * transform.up), -transform.right, Color.green, 1);
            RaycastHit2D HitObject = Physics2D.Raycast((Eye.position - 1.45f * transform.up), -transform.right, 1, Obstacles);
            if (HitObject)
            {
                Jump();
                velocity.x = 0;
            }
            else
            {
                velocity.x *= -1;
            }
        }
        else
        {
            //Debug.DrawRay((Eye.position - 1.45f * transform.up), transform.right, Color.green, 1);
            RaycastHit2D HitObject = Physics2D.Raycast((Eye.position - 1.45f * transform.up), transform.right, 1, Obstacles);
            if (HitObject)
            {
                Jump();
                velocity.x = 0;
            }
        }
        rb.linearVelocityX = velocity.x;
    }

    // Reads jump input
    public void Jump()
    {
        if (canJump)
        {
            // Debug.Log("jump performed");
            rb.linearVelocityY = jumpPower;
            canJump = false;
        }
    }

    // Processes gravity on the players
    public void ProcessGravity()
    {
        if (isGrounded)
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

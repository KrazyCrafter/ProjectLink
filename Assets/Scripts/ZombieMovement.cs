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
    [SerializeField] private float DestTimer;
    public bool SeeSpider;
    [SerializeField] private Transform Eye;
    [SerializeField] private LayerMask Obstacles;

    [SerializeField] private int LeftEdge;
    [SerializeField] private int RightEdge;

    private GameObject Spiderlion;
    [SerializeField] private LayerMask Visible;
    private float AttackTimer;
    private bool Attacked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Attacked = false;
        rb = GetComponent<Rigidbody2D>();
        Destination = Random.Range(LeftEdge, RightEdge);
        Spiderlion = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        SpySpider();
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
        if(Attacked)
        {
            AttackTimer += Time.deltaTime;
            if(AttackTimer > 1)
            {
                Attacked = false;
                AttackTimer = 0;
            }
        }
        if(SeeSpider && Vector2.Distance(transform.position, Spiderlion.transform.position) < 1.5f)
        {
            Attack();
        }
        else
        {
            ProcessMovement();
        }
        ProcessGravity();
    }
    private void SpySpider()
    {
        Debug.DrawRay(Eye.position, Vector2.Normalize(Spiderlion.transform.position - Eye.position), Color.green, 20f);
        RaycastHit2D HitObject = Physics2D.Raycast(Eye.position, Vector2.Normalize(Spiderlion.transform.position - Eye.position), 20, Visible);
        if(HitObject)
        {
            //Debug.Log("Saw " + HitObject.collider.name);
            if(HitObject.collider.gameObject == Spiderlion)
            {
                //Debug.Log("Saw Spider");
                SeeSpider = true;
                Destination = Spiderlion.transform.position.x;
                DestTimer = 0;
            }
            else
            {
                SeeSpider = false;
            }
        }
        else
        {
            SeeSpider = false;
        }
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

    public void Attack()
    {
        if (!Attacked)
        {
            Spiderlion.GetComponent<PlayerHealth>().TakeDamage(1);
            Attacked = true;
        }
    }
}

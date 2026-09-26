using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance {get; private set; }
    public Rigidbody2D playerRB;
    private bool isFacingRight = true;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float horizontalMovement;

    [Header("Dashing")]
    public float dashSpeed = 30f;
    public float dashDuration = 0.2f;
    public float dashCooldownTime = 0.15f;
    public float dashCooldownTimer;
    private bool startDashCooldownTimer;
    private bool isDashing;
    private bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Sprint")]
    public float sprintSpeed = 13f;
    private bool isSprinting;

    [Header("Jumping")]
    public float jumpPower = 13f;
    public int maxJumps = 2;
    private int jumpsRemaining;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float jumpCoyoteTime = 0.2f;
    private float jumpBufferTimer;
    private float jumpCoyoteTimer;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.9f, 0.05f);
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 14f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.05f, 0.9f);
    public LayerMask wallLayer;

    [Header("WallCling")]
    public float wallClingTime = 3f;
    private bool isWallClinging;
    private float wallClingTimer;

    [Header("WallMovement")]
    public float wallJumpTime = 0.5f;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(10f, 13f);

    [SerializeField] private float wallJumpBufferTime = 0.2f;
    [SerializeField] private float wallJumpCoyoteTime = 0.2f;
    private float wallJumpBufferTimer;
    private float wallJumpCoyoteTimer;

    [Header("Gliding")]
    public float glideSpeed = 2f;
    public bool isGliding = false;
    public bool canGlide;

    [Header("PlatformCheck")]
    public LayerMask platformLayer;

    [Header("PlatformDrop")]
    public Transform dropCheckPos;
    public Vector2 dropCheckSize = new Vector2(0.9f, 0.1375f);
    private bool canDrop;

    [Header("HouseScene")]
    public bool isHouseScene;

    [Header("Pause Movement")]
    public bool isPaused;

    [Header("Friction")]
    public PhysicsMaterial2D frictionless;
    public PhysicsMaterial2D friction;

    [Header("Animator")]
    public Animator rainAnimator;
    bool idleCountdown = true;
    bool isLongIdle = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        
        // if (SaveManager.Instance.SaveFile.ProgressionFlag == 0) // Sets the jumps at the start of a scene based on the progression flag
        {
            maxJumps = 1;
        }
        // else
        {
            // maxJumps = 2;
        }

        // if (SaveManager.Instance.SaveFile.ProgressionFlag >= 4)
        {
            // canGlide = true;
        }

        isPaused = false;
        // playerRB.sharedMaterial = frictionless;

        Cursor.visible = false;
    }

    void FixedUpdate()
    {      
        if (isPaused || isDashing)
            return;
        
        GroundCheck();
        ProcessPlatformDrop();
        ProcessJumpForgiveness();
        ProcessGravity();
        if (!isHouseScene)
        {
            ProcessWallClinging();
            ProcessWallJump();
        }
        ProcessDashTimer();

        // Debug.Log($"Wall Cling: {isWallClinging}, {wallClingTimer}");
        // Debug.Log($"Wall Jump: {isWallJumping}, {wallJumpTimer}");

        ProcessMovement();

        if (!isWallJumping)
        {
            FlipSprite();
        }
        // Debug.Log(playerRB.linearVelocityX);
    
        if (idleCountdown == true && !isLongIdle)
        {
            idleCountdown = false;
            isLongIdle = true;
            StartCoroutine("LongIdle");
        }

        // Running the dev script only in the editor
        #if UNITY_EDITOR
        // DevScript.Update();
        #endif
    }

    /// <summary>
    /// Processes the coyote time and input buffer for jumping
    /// </summary>
    private void ProcessJumpForgiveness()
    {
        // Coyote Time
        if (isGrounded)
        {
            jumpCoyoteTimer = jumpCoyoteTime;
        }
        else
        {
            jumpCoyoteTimer -= Time.deltaTime;
        }
        if (isWallClinging)
        {
            wallJumpCoyoteTimer = wallJumpCoyoteTime;
        }
        else
        {
            wallJumpCoyoteTimer -= Time.deltaTime;
        }

        // Half of Input Buffer (See Jump method)
        jumpBufferTimer -= Time.deltaTime;  
        wallJumpBufferTimer -= Time.deltaTime;  
    }

    private void ProcessDashTimer()
    {
        if (isGrounded)
        {
            startDashCooldownTimer = true;
        }
        if (startDashCooldownTimer & dashCooldownTimer > 0 & !canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        else if (isWallClinging || startDashCooldownTimer & dashCooldownTimer <= 0)
        {
            startDashCooldownTimer = false;
            dashCooldownTimer = 0f;
            canDash = true;
        }
    }

    public void Move(InputAction.CallbackContext action)
    {
        if(isPaused) 
            return;

        rainAnimator.SetBool("isLongIdle", false);

        horizontalMovement = action.ReadValue<Vector2>().x;

        //animation
        if (horizontalMovement != 0.0f)
        {
            isLongIdle = false;
            rainAnimator.SetBool("isRunning", true);
        }
        else
        {
            rainAnimator.SetBool("isRunning", false);
        }
    }

    public void Dash(InputAction.CallbackContext action)
    {
        if(isPaused) 
            return;
        
        if (action.performed & canDash & !isGrounded & !isHouseScene)
        {
            StartCoroutine(DashCoroutine());
        }
        else if (action.performed & isGrounded)
        {
            isSprinting = true;
        }
        else if (action.canceled)
        {
            isSprinting = false;
        }
    }

    /// <summary>
    /// Handles the Dashing logic
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;

        float dashDirection;
        if(horizontalMovement != 0.0f && !isWallJumping)
            dashDirection = horizontalMovement == 1.0f ? 1f : -1f;
        else
            dashDirection = isFacingRight ? 1f : -1f;

        playerRB.linearVelocityX = dashDirection * dashSpeed;
        playerRB.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(dashDuration);

        playerRB.linearVelocityX = 0f;
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;

        isDashing = false;
        trailRenderer.emitting = false;

        startDashCooldownTimer = false;
        dashCooldownTimer = dashCooldownTime;
    }

    public void Drop(InputAction.CallbackContext action)
    {
        if(action.performed)
        {
            canDrop = true;
        }
        else if(action.canceled)
        {
            canDrop = false;
        }
    }

    /// <summary>
    /// Processes the player's drops through platforms. 
    /// </summary>
    private void ProcessPlatformDrop()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(platformLayer);
        List<Collider2D> dropColliderOverlaps = new List<Collider2D>(10);
        int count = Physics2D.OverlapBox(dropCheckPos.position, dropCheckSize, 0, filter, dropColliderOverlaps);
        for(int i = 0; i < count; i++) 
        {
            GameObject platformObj = dropColliderOverlaps[i].gameObject;
            if (canDrop && platformObj.CompareTag("Platform"))
            {
                Collider2D platformCollider = platformObj.GetComponent<Collider2D>();
                platformCollider.isTrigger = true;
            }
        }
    }

    public void Glide(InputAction.CallbackContext action)
    {
        if(canGlide && !isGrounded && !isWallJumping && !isWallClinging && !isDashing && action.performed)
        {
            isGliding = true;
            rainAnimator.SetBool("isGliding", true);
        }
        else if(action.canceled)
        {
            isGliding = false;
            rainAnimator.SetBool("isGliding", false);
        }
    }

    /// <summary>
    /// Reenables the platform collision after player drops through and exits.
    /// </summary>
    /// <param name="collision">The collider of the platform.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
            GameObject collisionObj = collision.gameObject;
            if (collisionObj.CompareTag("Platform"))
            {
                Collider2D platformCollider = collisionObj.GetComponent<Collider2D>();
                platformCollider.isTrigger = false;
            }
    }

    public void Jump(InputAction.CallbackContext action)
    {
        if(isPaused) 
            return;

        // Half of jump buffer (See ProcessJumpForgivness method)
        if (action.performed)
        {
            jumpBufferTimer = jumpBufferTime;
            wallJumpBufferTimer = wallJumpBufferTime;
        }

        // Handles the player's wall jumping logic
        if ((wallJumpBufferTimer > 0.0f && wallJumpCoyoteTimer > 0.0 || action.performed && (WallCheck() || PlatformCheck(wallCheckPos, wallCheckSize))) && wallJumpTimer > 0)
        {
            isWallJumping = true;
            playerRB.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); // Player jumps away from the wall.
            wallJumpTimer = 0f;
            wallClingTimer = wallClingTime;
            wallJumpBufferTimer = 0;
            wallJumpCoyoteTimer = 0;

            // Flips the player around for wall jumping
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight ^= true;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime);
        }

        // Handles the player's jumping logic
        if (jumpsRemaining > 0 && !isWallJumping && !isDashing)
        {
            if ((jumpBufferTimer > 0.0f && jumpCoyoteTimer > 0.0f) || action.performed)
            {
                playerRB.linearVelocityY = jumpPower;
                jumpsRemaining--;
                jumpBufferTimer = 0;
                jumpCoyoteTimer = 0;
                // Debug.Log($"Normal jump: {jumpsRemaining}/{maxJumps}");
            }
            else if (action.canceled)
            {
                playerRB.linearVelocityY *= 0.5f;
                jumpsRemaining--;
                jumpBufferTimer = 0;
                jumpCoyoteTimer = 0;
                // Debug.Log($"Half jump: {jumpsRemaining}/{maxJumps}");
            }
            rainAnimator.SetBool("isLongIdle", false); isLongIdle = false;
            rainAnimator.SetBool("isJumping", true);
        }
    }

    public void Pause(InputAction.CallbackContext action)
    {
        // if(DialogueHolder.Instance.CheckIsOver() && action.performed)
        {
            // PauseGame();
        }
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        isPaused ^= true;
        // Debug.Log("Level");
        // if(SettingsMenuManager.Instance.settingsMenu.activeSelf) 
            // PauseMenuManager.Instance.HidePauseMenu();
        // else
            // PauseMenuManager.Instance.ShowHidePauseMenu();
        // SettingsMenuManager.Instance.HideSettingsMenu();
        Cursor.visible = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    /// <summary>
    /// Checks to see if the player is touching the ground and resets the remaining jumps to max.
    /// </summary>
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer) || PlatformCheck(groundCheckPos, groundCheckSize))
        {
            jumpsRemaining = maxJumps;
            // Debug.Log($"{jumpsRemaining}/{maxJumps}");
            playerRB.sharedMaterial = friction;
            isGrounded = true;
            rainAnimator.SetBool("isJumping", false);
            rainAnimator.SetBool("isGliding", false);

            if (horizontalMovement == 0.0f)
            {
                idleCountdown = true;
            }
        }
        else
        {
            playerRB.sharedMaterial = frictionless;
            isGrounded = false;
            rainAnimator.SetBool("isLongIdle", false); isLongIdle = false;
            rainAnimator.SetBool("isJumping", true);
        }
    }

    /// <summary>
    /// Applies gravitational force to the player sprite.
    /// </summary>
    private void ProcessGravity()
    {

        if (!isWallClinging)
        {
            if (playerRB.linearVelocityY < 0)
            {
                playerRB.gravityScale = baseGravity * fallSpeedMultiplier;
                if(isGliding && !isHouseScene)
                    playerRB.linearVelocityY = Mathf.Max(playerRB.linearVelocityY, -glideSpeed);  
                else
                    playerRB.linearVelocityY = Mathf.Max(playerRB.linearVelocityY, -maxFallSpeed);   
            }
            else
            {
                playerRB.gravityScale = baseGravity;
            }
        }
        else
        {
            playerRB.gravityScale = 0f;
        }
    }

    /// <summary>
    /// Flips the player sprite if the player changes direction.
    /// </summary>
    private void FlipSprite()
    {
        if (isFacingRight & horizontalMovement < 0 || !isFacingRight & horizontalMovement > 0)
        {
            isFacingRight ^= true;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    /// <summary>
    /// Applies a horizontal force in the direction of the player's move input.
    /// </summary>
    private void ProcessMovement()
    {
        if(isWallJumping) 
        {
            isSprinting = false;
            float velocityX = 1.0f;
            float velocityY = 1.0f;
            if(isFacingRight)
            {
                if(horizontalMovement > 0.0f)
                    velocityX = 1.015f;
                else if(horizontalMovement < 0.0f)
                    velocityX = 0.985f;
                playerRB.linearVelocityX *= velocityX;
                playerRB.linearVelocityX = Mathf.Min(playerRB.linearVelocityX, wallJumpPower.x);
                
                if(horizontalMovement > 0.0f)
                    velocityY = 1.001f;
                else if(horizontalMovement < 0.0f)
                    velocityY = 0.999f;
                playerRB.linearVelocityY *= velocityY;
                playerRB.linearVelocityY = Mathf.Min(playerRB.linearVelocityY, wallJumpPower.y);
            }
            else
            {
                if(horizontalMovement < 0.0f)
                    velocityX = 1.015f;
                else if(horizontalMovement > 0.0f)
                    velocityX = 0.985f;
                playerRB.linearVelocityX *= velocityX;
                playerRB.linearVelocityX = Mathf.Max(playerRB.linearVelocityX, -wallJumpPower.x);
                
                if(horizontalMovement < 0.0f)
                    velocityY = 1.001f;
                else if(horizontalMovement > 0.0f)
                    velocityY = 0.999f;
                playerRB.linearVelocityY *= velocityY;
                playerRB.linearVelocityY = Mathf.Max(playerRB.linearVelocityY, -wallJumpPower.y);
            }
            // Debug.Log("velocityX: " + velocityX);
            // Debug.Log("velocityY: " + velocityY);
        }
        else if(isSprinting)
            playerRB.linearVelocityX = horizontalMovement * sprintSpeed;
        else
            playerRB.linearVelocityX = horizontalMovement * moveSpeed;
    }

    void OnDrawGizmosSelected()
    {
        // Draws GroundCheck in green.
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        // Draws WallCheck in blue.
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);

        // Draws DropCheck in magenta.
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(dropCheckPos.position, dropCheckSize);
    }

    /// <summary>
    /// Checks to see if the player is touching a platform.
    /// </summary>
    /// <param name="checkPos"></param>
    /// <param name="checkSize"></param>
    /// <returns></returns>
    private bool PlatformCheck(Transform checkPos, Vector2 checkSize)
    {
        return Physics2D.OverlapBox(checkPos.position, checkSize, 0, platformLayer);
    }

    /// <summary>
    /// Checks to see if the player is touching the wall.
    /// </summary>
    /// <returns></returns>
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    /// <summary>
    /// Processes the player's wall clinging logic
    /// </summary>
    void ProcessWallClinging()
    {
        if (isGrounded)
        {
            wallClingTimer = wallClingTime;
        }
        if (isWallClinging)
        {
            wallClingTimer -= Time.deltaTime;
        }

        if (!isGrounded & (WallCheck() || PlatformCheck(wallCheckPos, wallCheckSize)) & horizontalMovement != 0 & wallClingTimer > 0f)
        {
            isWallClinging = true;
            playerRB.linearVelocityY = 0f;
            rainAnimator.SetBool("isLongIdle", false); isLongIdle = false;
            rainAnimator.SetBool("isWallClinging", true);
        }
        else
        {
            isWallClinging = false;
            rainAnimator.SetBool("isWallClinging", false);
        }
    }

    /// <summary>
    /// Processes whether the player can wall jump
    /// </summary>
    private void ProcessWallJump()
    {
        if (isWallClinging)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
        // Debug.Log("CancelWallJump has triggerd.");
    }

    public bool IsDashing
    {
        get => isDashing;
    }

    public bool GetIsFacingRight()
    {
        return isFacingRight;
    }

    IEnumerator LongIdle()
    {
        yield return new WaitForSeconds(3);
        rainAnimator.SetBool("isLongIdle", true);
    }
}

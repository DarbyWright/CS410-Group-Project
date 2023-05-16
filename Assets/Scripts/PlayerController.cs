using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour {

    // Game manager
    GameManager gameManager;

    // UI for debug
    //public TextMeshProUGUI UI_Debug;

    // Rigidbody
    Rigidbody myRB;

    // Input
    float horizontalInput;
    float verticalInput;

    // For some reason Input.GetKey stopped working, so we're going wiht Input.GetButton for now

    // Key mapping
    public KeyCode runKey   = KeyCode.LeftShift;
    public KeyCode jumpKey  = KeyCode.Space;
    public KeyCode dashKey  = KeyCode.Z;
    public KeyCode glideKey = KeyCode.X;

    // Unlockalble abilities
    public bool canDoubleJump = false;
    public bool canDash       = false;
    public bool canGlide      = false;

    // Ground check
    LayerMask groundLayerMask;
    bool onGround = false;
    float groundCheckDistance = 1f;
    //Adds raycasts slightly in each direction around the player to help with uneven environments
    //Probably will not be necessary if we switch to a controller
    private static readonly Vector3[] raycastPositions = new Vector3[]
    {
    new Vector3(0.5f, 0f, 0f),
    new Vector3(-0.5f, 0f, 0f),
    new Vector3(0f, 0f, 0.5f),
    new Vector3(0f, 0f, -0.5f),
    Vector3.zero 
    };

    // Gravity
    static float globalGravity  = -9.81f;
    float gravityScale          = 1.0f;

    // Moving
    Vector3 moveDirection;
    float targetSpeed = 0f;
    float moveSpeed   = 0f;
    float minSpeedDif = 0.03125f; // 1/32
    
    // New Code From Darby
    public Vector2 turn;
    public float rotationSpeed = 0.1f;

    // Accleration
    float acc       = 0.1f;
    float friction  = 0.97f;
    float airAcc    = 0.08f;
    float airFric   = 0.94f;

    // Walking and running
    float walkSpeed = 3f;
    float runSpeed  = 6f;
    //float airSpeed  = 1f;

    // Jumping
    bool didDoubleJump  = false;
    float jumpStrength  = 24f;


    // Dashing
    bool dashing        = false;
    float dashStrength  = 40f;
    int dashInit        = 0;
    int dashInitMax     = Mathf.RoundToInt(1.25f * 60f);
    int dashCooldown    = 0;
    int dashCooldownMax = 3 * 60;

    // Gliding
    bool gliding        = false;
    float glideSpeedCut = 0.5f;

    // Gravity
    float jumpPeakMin   = 2f;
    float jumpGrav      = 5f;
    float hangtimeGrav  = 3f;
    float fallGrav      = 7f;
    float glideGrav     = 0.5f;

    // Death and respawn location
    float deathPlane = -30f;
    Vector3 respawnPos;

    //Animation variables
    public Animator animator;




    /* * * * * * * */

    // Initialize
    public void Awake() {
        
        // NEW
        Cursor.lockState = CursorLockMode.Locked;

        // Get game manager for global variables
        gameManager = FindObjectOfType<GameManager>();

        // Get rigidbody for forces
        myRB = GetComponent<Rigidbody>();

        // Player always stays upright
        myRB.freezeRotation = true;

        // Player can jump on all objects that are on the "Ground" layer
        groundLayerMask = LayerMask.GetMask("Ground");

        // Set respawn point
        respawnPos = transform.position;

        // Get abilities if the game manager has tracked them as collected
        canDoubleJump = gameManager.hasDoubleJump;
        canDash = gameManager.hasDash;
        canGlide = gameManager.hasGlide;
    }


    // Every frame - start input stuff
    public void Update() {

        updateMovementAnimation();

        // Freeze right before dash
        if (dashInit > 0) dashInit--;
        if (dashInit == 1) Dash();

        // Time until can dash again
        if (dashCooldown > 0) dashCooldown--;
        else dashing = false;

        onGround = Grounded();
        
        if (onGround) {
            gliding = false;
            didDoubleJump = false;
        }

        // Get input and update movement - player has no control while dash begins
        if (dashInit == 0)
            GetInput();

        // Player falls too far and dies
        if (transform.position.y < deathPlane)
            Die();
    }

    // Every frame, but after physics calculations - actual motion
    public void FixedUpdate() {

        // Apply gravity
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        myRB.AddForce(gravity, ForceMode.Acceleration);

        // Move player
        if (dashInit == 0)
            MovePlayer();
        // Player freezes right before a dash
        else
            gravityScale = 0f;


        // UI for debug
        //Temp commented out to prevent the need for Canvas in every scene for now
        /*
        UI_Debug.text = "- DEBUG -\n";
        UI_Debug.text += "target speed\n";
        UI_Debug.text += "  X: " + (moveDirection.x * targetSpeed).ToString() + "\n";
        UI_Debug.text += "  Y: " + myRB.velocity.y.ToString() + "\n";
        UI_Debug.text += "  Z: " + (moveDirection.z * targetSpeed).ToString() + "\n";
        UI_Debug.text += "speed\n";
        UI_Debug.text += "  X: " + (moveDirection.x * moveSpeed).ToString() + "\n";
        UI_Debug.text += "  Y: " + myRB.velocity.y.ToString() + "\n";
        UI_Debug.text += "  Z: " + (moveDirection.z * moveSpeed).ToString() + "\n";
        UI_Debug.text += "grav: " + gravityScale.ToString() + "\n";
        UI_Debug.text += "jumpBuffer: " + jumpBuffer.ToString() + "\n";
        UI_Debug.text += "coyoteTime: " + coyoteTime.ToString() + "\n";
        UI_Debug.text += "dashInit: " + dashInit.ToString() + "\n";
        UI_Debug.text += "dashCooldown: " + dashCooldown.ToString() + "\n";
        */
    }

    void updateMovementAnimation()
    {
        //New code from Jesse will look to improve this but with the correct speed setting makes it easy to enable the idle animation
        if(horizontalInput == 0 && verticalInput == 0)
        {
            animator.SetFloat("currentSpeed", moveSpeed - walkSpeed);
        }
        else
        {
            animator.SetFloat("currentSpeed", moveSpeed);
        }
        
    }


    // Player input - use vars for keys for potential custom button mapping
    void GetInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Walking
        targetSpeed = walkSpeed;

        // Running
        if (Input.GetKey(runKey))
            targetSpeed = runSpeed;

        // Jumping
        if (Input.GetKeyDown(jumpKey) || Input.GetButtonDown("Jump"))
            Jump();

        // Dashing
        if (Input.GetKeyDown(dashKey) && canDash && dashCooldown == 0)
            dashInit = dashInitMax;

        // Gliding
        gliding = Input.GetKey(glideKey) && canGlide && onGround == false;

        // At the start of a glide, reduce falling speed
        if (Input.GetKeyDown(glideKey) && canGlide) {
            Vector3 vel = myRB.velocity;
            myRB.velocity = new Vector3(vel.x, glideSpeedCut * vel.y, vel.z);
        }

    }


    // Move player - acceleration, momentum, air resistance, jump gravity arc, etc
    void MovePlayer() {

        // If really close to target speed, just go at target speed
        if (Mathf.Abs(moveSpeed - targetSpeed) < minSpeedDif)
            moveSpeed = targetSpeed;

        // If really close to stopping, just stop
        if (Mathf.Abs(moveSpeed) < minSpeedDif)
            moveSpeed = 0f;

        // Ground physics
        if (onGround) {

            // Acceleration
            if (moveSpeed != targetSpeed)
                moveSpeed += acc * Mathf.Sign(targetSpeed);

            // Friction
            if (Mathf.Abs(moveSpeed) > Mathf.Abs(targetSpeed))
                moveSpeed *= friction;

            // Default gravity
            gravityScale = fallGrav;
        }

        // Air physics
        else {

            // Acceleration
            if (moveSpeed != targetSpeed)
                moveSpeed += airAcc * Mathf.Sign(targetSpeed);

            // Friction
            if (Mathf.Abs(moveSpeed) > Mathf.Abs(targetSpeed))
                moveSpeed *= airFric;

            // Change gravity for more "player friendly" jump arc

            // Get vertical speed
            float ySpeed = myRB.velocity.y;

            // Gravity at beginning of jump arc
            gravityScale = jumpGrav;

            // Gravity at peak of jump arc
            if (ySpeed < jumpPeakMin)
                gravityScale = hangtimeGrav;

            // Gravity at end of jump arc
            if (ySpeed < -jumpPeakMin)
                gravityScale = fallGrav;
        }

        // Gliding gravity
        if (gliding)
            gravityScale = glideGrav;

            


        // Player Movement is now in relation to the camera position instead of world space
        var camera = Camera.main;
        var forward = camera.transform.forward;
        var right = camera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        moveDirection = forward * verticalInput + right * horizontalInput;

        //Get direction camera is facing
        Quaternion cameraDirection = Quaternion.LookRotation(forward);

        //Set player rotation to match, have tried slowly rotating but it always seems to cause tearing because of how fast you can look around
        transform.rotation = cameraDirection;
        transform.position += (moveDirection * moveSpeed * Time.deltaTime);


    }


    // Jump motion and sound
    void Jump() {

        if(onGround || (!didDoubleJump && canDoubleJump))
        {
            // Set gravity to start of jump arc
            gravityScale = jumpGrav;

            // Apply force
            Vector3 vel = myRB.velocity;
            myRB.velocity = new Vector3(vel.x, 0f, vel.z);
            myRB.AddForce(jumpStrength * Vector3.up, ForceMode.Impulse);
            // Jump sound
            AudioManager audioManager = FindObjectOfType<AudioManager>();

            if (!onGround && canDoubleJump)
            {
                //audioManager.Play("SFX_PlayerDoubleJump");
                animator.CrossFade("DoubleJump", 0);
                didDoubleJump = true;
            }
            else
            {
                //audioManager.Play("SFX_PlayerJump");
                animator.CrossFade("StartJump", 0);
            }


        }

    }


    // Dash motion and sound
    void Dash() {
        dashing = true;

        // When can dash again
        dashCooldown = dashCooldownMax;

        // Move fast
        if (dashing && horizontalInput + verticalInput > 0f) {
            targetSpeed += dashStrength * Mathf.Sign(targetSpeed);
            moveSpeed = targetSpeed;
        }

        // Dash sound
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
            audioManager.Play("SFX_PlayerDash");
    }


    // Triggers
    void OnTriggerEnter(Collider other) {

        // DoubleJump item
        if (other.gameObject.CompareTag("ItemDoubleJump")) {
            gameManager.GotDoubleJump();
            canDoubleJump = true;
            // Temperarily just set to inactive to make the hovering text work
            other.gameObject.SetActive(false);
            SetCheckPoint(other.transform.position.x, other.transform.position.y, other.transform.position.z);
            // Destroy(other.gameObject);

        }
        // Dash item
        if (other.gameObject.CompareTag("ItemDash")) {
            gameManager.GotDash();
            canDash = true;
            // Temperarily just set to inactive to make the hovering text work
            other.gameObject.SetActive(false);
            // Destroy(other.gameObject);
        }
        // Glide item
        if (other.gameObject.CompareTag("ItemGlide")) {
            gameManager.GotGlide();
            canGlide = true;
            // Temperarily just set to inactive to make the hovering text work
            other.gameObject.SetActive(false);
            // Destroy(other.gameObject);
        }

        // Checkpoints
        if (other.gameObject.CompareTag("Checkpoint")) {
            other.gameObject.SetActive(false);
            float x = other.gameObject.transform.position.x;
            float y = other.gameObject.transform.position.y;
            float z = other.gameObject.transform.position.z;
            SetCheckPoint(x, y, z);
        }
    }

    //New Code from Jesse, Updated groundCheck to have 5 raycasts, one in each Cardinal and then the center one still
    bool Grounded()
    {
        //Check each position if one hits then early return to help performance
        for (int i = 0; i < raycastPositions.Length; i++)
        {
            onGround = Physics.Raycast(transform.position + raycastPositions[i], Vector3.down, groundCheckDistance, groundLayerMask);
            if (onGround)
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("OutOfBounds"))
        {
            Die();
        }
    }

    // Update the player's respawn position
    void SetCheckPoint(float x, float y, float z) {
        respawnPos = new Vector3(x, y, z);
    }


    // Die and respawn
    void Die() {

        // Update death counter
        gameManager.PlayerDeath();

        // Reset vars
        moveSpeed = 0f;
        targetSpeed = 0f;
        didDoubleJump = false;
        dashing = false;
        dashInit = 0;
        dashCooldown = 0;
        gravityScale = fallGrav;

        // Set position
        transform.position = respawnPos;
    }
}

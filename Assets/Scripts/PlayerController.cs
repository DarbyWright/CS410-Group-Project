using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour {

    // Game and audio managers
    GameManager gameManager;
    AudioManager audioManager;

    // UI for debug
    public TextMeshProUGUI UI_Debug;

    // Rigidbody
    Rigidbody myRB;

    // Whether the player can be controlled on not (pausing, death animation, cutscenes, etc)
    public bool active = true;

    // Input
    float horizontalInput;
    float verticalInput;

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
    bool onGround             = false;
    float groundCheckDistance = 1f;

    //Adds raycasts slightly in each direction around the player to help with uneven environments
    //Probably will not be necessary if we switch to a controller
    private static readonly Vector3[] raycastPositions = new Vector3[] {
        new Vector3(0.5f, 0.5f, 0f),
        new Vector3(-0.5f, 0.5f, 0f),
        new Vector3(0f, 0.5f, 0.5f),
        new Vector3(0f, 0.5f, -0.5f),
        Vector3.zero
    };

    // Gravity
    static float globalGravity  = -9.81f;
    float gravityScale          = 1.0f;

    // Moving
    Vector3 moveDirection;
    bool moving       = false;
    float moveSpeed   = 0f;
    float targetSpeed = 0f;
    float minSpeedDif = 0.125f;
    float ySpeed      = 0f;
    //float turnSpeed   = 400.0f;

    // Accleration
    float acc       = 0.1f;
    float friction  = 0.97f;
    float airAcc    = 0.08f;
    float airFric   = 0.94f;

    // Walking and running
    float walkSpeed = 4f;
    float runSpeed  = 7f;

    // Jumping
    bool jumping             = false;
    bool didDoubleJump       = false;
    float jumpStrength       = 24f;
    float doubleJumpStrength = 24f;
    float jumpSpeedCut       = 0.5f;
    int earlyJumpWindow      = 0;
    int earlyJumpWindowMax   = 2 * 60;
    int lateJumpWindow       = 0;
    int lateJumpWindowMax    = 2 * 60;

    // Dashing
    bool dashing        = false;
    float dashStrength  = 40f;
    int dashInit        = 0;
    int dashInitMax     = Mathf.RoundToInt(0.75f * 60f);
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

    // Death, animation time, and respawn location
    float deathPlane      = -50f;
    int deathAnimTimer    = 0;
    int deathAnimTimerMax = 2 * 60;
    Vector3 respawnPos;

    // Animation variables
    CharacterController controller;
    public Animator animator;


    /* * * * * * * */


    // Initialize
    public void Awake() {

        // NEW
        Cursor.lockState = CursorLockMode.Locked;

        // Get game and audio managers
        gameManager  = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();

        // Get controller and animator
        //controller = GetComponent <CharacterController>();
  			//animator = gameObject.GetComponentInChildren<Animator>();

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
        canDash       = gameManager.hasDash;
        canGlide      = gameManager.hasGlide;

        //////////////////////////////////////////////////////////////////////////////////// Delete this after testing
        gameManager.GotDoubleJump();                            // for alllowing dbl jumping in hubworld
        canDoubleJump = true;
        //////////////////////////////////////////////////////////////////////////////////////////////////
    }


    // Every frame - start input stuff
    public void Update() {

        // Animation
        updateMovementAnimation();

        // Timers
        handleTimers();

        // Ground check
        onGround = Grounded();

        // Get input and update movement if allowed to
        if (active)
            GetInput();

        // Player falls too far and dies
        if (transform.position.y < deathPlane)
            DieAndRespawn();
    }


    // New code from Jesse will look to improve this but with the correct speed setting makes it easy to enable the idle animation
    void updateMovementAnimation() {

        // Not moving, don't animation
        if (!moving) {
            animator.SetFloat("currentSpeed", 0f); // moveSpeed - walkSpeed
        }

        // Moving, animate
        else {
            animator.SetFloat("currentSpeed", moveSpeed);
        }
    }


    // Timers for jump input window, dash charge and cooldown, and death animation
    void handleTimers() {

        // Reduce the jump time window
        if (earlyJumpWindow > 0) earlyJumpWindow--;
        if (lateJumpWindow > 0) lateJumpWindow--;

        // Freeze right before dash
        if (dashInit > 0) dashInit--;

        // At the end of the dash "charge" give control back to the player
        if (dashInit == 1) {
            active = true;
            Dash();
        }

        // Time until can dash again
        if (dashCooldown > 0) dashCooldown--;
        else dashing = false;

        // Freeze right before dash
        if (dashInit > 0) dashInit--;

        // Death animation / cutscene
        if (deathAnimTimer > 0) deathAnimTimer--;

        // At the end of the death animation, die and respawn
        if (deathAnimTimer == 1)
            DieAndRespawn();
    }


    // New Code from Jesse, Updated groundCheck to have 5 raycasts, one in each Cardinal and then the center one still
    bool Grounded() {

        // Check each position if one hits then early return to help performance
        for (int i = 0; i < raycastPositions.Length; i++) {

            // Recast to check for ground - REMOVED LAYER MASK
            onGround = Physics.Raycast(transform.position + raycastPositions[i], Vector3.down, groundCheckDistance); //groundLayerMask

            // TEMPORARY - use controller check so don't have to use ground tag
            //onGround = onGround || controller.isGrounded;

            // Reset some vars and confrim player is on ground
            if (onGround) {
                jumping = false;
                gliding = false;
                didDoubleJump = false;
                lateJumpWindow = lateJumpWindowMax;
                return true;
            }
        }
        return false;
    }


    // Player input - use vars for keys for potential custom button mapping
    void GetInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        moving = horizontalInput != 0 || verticalInput != 0;

        // Walking
        targetSpeed = walkSpeed;

        // Running
        if (Input.GetKey(runKey))
            targetSpeed = runSpeed;

        // Jumping - track that the jump button was pressed
        if (Input.GetKeyDown(jumpKey) && (onGround || (canDoubleJump && !didDoubleJump))) {
            // earlyJumpWindow = earlyJumpWindowMax;
            if (onGround)
                Jump(jumpStrength);
            else
                Jump(doubleJumpStrength);
        }

        // When release jump, reduce upward speed
        Vector3 vel = myRB.velocity;
        if (Input.GetKeyUp(jumpKey)) {
            jumping = false;
            myRB.velocity = new Vector3(vel.x, jumpSpeedCut * vel.y, vel.z);
            //ySpeed *= jumpSpeedCut;
        }

        // Dashing
        if (Input.GetKeyDown(dashKey) && canDash && dashCooldown == 0)
            dashInit = dashInitMax;

        // Gliding
        gliding = Input.GetKey(glideKey) && canGlide && !onGround; //&& ySpeed < 0f; //myRB.velocity.y < 0f

        // At the start of a glide, reduce falling speed
        if (Input.GetKeyDown(glideKey) && canGlide && vel.y < 0) {
            //Vector3 vel = myRB.velocity;
            //myRB.velocity = new Vector3(vel.x, glideSpeedCut * vel.y, vel.z);
            ySpeed *= glideSpeedCut;
        }
    }


    // Every frame, but after physics calculations - actual motion
    public void FixedUpdate() {

        // Apply gravity
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        myRB.AddForce(gravity, ForceMode.Acceleration);

        // Move player
        if (active)
            MovePlayer();

        // Player has no control and therefore also no gravity
        else
            gravityScale = 0f;
    }


    // Move player - acceleration, momentum, air resistance, jump gravity arc, etc
    void MovePlayer() {

        // If really close to target speed, just go at target speed
        if (Mathf.Abs(moveSpeed - targetSpeed) < minSpeedDif)
            moveSpeed = targetSpeed;

        // If really close to stopping, just stop
        if (Mathf.Abs(moveSpeed) < minSpeedDif && !moving)
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
            if (Mathf.Abs(moveSpeed) < Mathf.Abs(targetSpeed))
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
            if (ySpeed < jumpPeakMin && Mathf.Abs(ySpeed) > minSpeedDif)
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
        transform.position += (moveDirection.normalized * moveSpeed * Time.deltaTime);

        // If the player can jump, pressed jump, and (was just on the ground or can double jumping)
        // earlyJumpWindow - used instead of checking the jump button so players still jump if they press too early
        // lateJumpWindow - used instead of checking for onGround so players still jump if they press too late
        if (!jumping && earlyJumpWindow > 0 && (lateJumpWindow > 0 || (canDoubleJump && !didDoubleJump))) {
            if (onGround)
                Jump(jumpStrength);
            else
                Jump(doubleJumpStrength);
        }
    }


    // Jump motion and sound
    void Jump(float strength) {
        jumping = true;

        // Set gravity to start of jump arc
        gravityScale = jumpGrav;

        // Apply force
        Vector3 vel = myRB.velocity;
        myRB.velocity = new Vector3(vel.x, 0f, vel.z);
        myRB.AddForce(strength * Vector3.up, ForceMode.Impulse);

        // Double jumped
        if (!onGround && canDoubleJump)
            didDoubleJump = true;

        // Animation
        if (!didDoubleJump)
            animator.CrossFade("StartJump", 0);
        else
            animator.CrossFade("DoubleJump", 0);

        // Sound
        if (audioManager != null) {

            // Single
            if (!didDoubleJump)
                audioManager.PlaySFX("SFX_PlayerJump");
            // Double
            else
                audioManager.PlaySFX("SFX_PlayerDoubleJump");
        }
    }


    // Dash motion and sound
    void Dash() {
        dashing = true;

        // When can dash again
        dashCooldown = dashCooldownMax;

        // Move fast
        if (dashing && moving) {
            targetSpeed += dashStrength * Mathf.Sign(targetSpeed);
            moveSpeed = targetSpeed;
        }

        // Dash sound
        if (audioManager != null)
            audioManager.PlaySFX("SFX_PlayerDash");
    }


    // Bounce motion and sound
    public void Bounce(float bounceDefaultStrength, float bounceJumpStrength) {
        didDoubleJump = false;

        // Default bounce
        if (Input.GetKey(jumpKey) == false)
            Jump(bounceDefaultStrength);

        // If holding jump
        else
            Jump(bounceJumpStrength);
    }


    // Triggers
    void OnTriggerEnter(Collider other) {

        // Music trigger
        if (other.gameObject.CompareTag("MusicTrigger")) {
            gameManager.GotDoubleJump();
            canDoubleJump = true;
            //maxJumps = 2;
            // Temperarily just set to inactive to make the hovering text work
            other.gameObject.SetActive(false);

        }

        // DoubleJump item
        if (other.gameObject.CompareTag("ItemDoubleJump")) {
            gameManager.GotDoubleJump();
            canDoubleJump = true;
            // Temperarily just set to inactive to make the hovering text work
            other.gameObject.SetActive(false);
            SetCheckPoint(other.transform.position.x, other.transform.position.y, other.transform.position.z, false);
            // Destroy(other.gameObject);

            // Item Jingle
            if (audioManager != null)
                audioManager.PlaySFX("SFX_SpecialEvent");
        }

        // Dash item
        if (other.gameObject.CompareTag("ItemDash")) {
            gameManager.GotDash();
            canDash = true;
            // Temporarily just set to inactive to make the hovering text work
            other.gameObject.SetActive(false);
            SetCheckPoint(other.transform.position.x, other.transform.position.y, other.transform.position.z, false);
            // Destroy(other.gameObject);
        }

        // Glide item
        if (other.gameObject.CompareTag("ItemGlide")) {
            gameManager.GotGlide();
            canGlide = true;
            // Temperarily just set to inactive to make the hovering text work
            other.gameObject.SetActive(false);
            SetCheckPoint(other.transform.position.x, other.transform.position.y, other.transform.position.z, false);
            // Destroy(other.gameObject);
        }

        // Checkpoints
        if (other.gameObject.CompareTag("Checkpoint")) {
            //other.gameObject.SetActive(false);
            float x = other.gameObject.transform.position.x;
            float y = other.gameObject.transform.position.y;
            float z = other.gameObject.transform.position.z;

            // Only set checkpoint if this is a differnet one (for sound or animation if we want)
            if (!(x == respawnPos.x && y == respawnPos.y & z == respawnPos.z))
                SetCheckPoint(x, y, z, true);
        }
    }


    // Collide with objects
    void OnCollisionEnter(Collision collision) {

        // If a projectile hits player, die
        if (collision.gameObject.CompareTag("Enemy") || 
            collision.gameObject.CompareTag("EnemyProjectile") ||
            collision.gameObject.CompareTag("OutOfBounds")) {
            DeathAnim();
        }
    }


    // Update the player's respawn position
    void SetCheckPoint(float x, float y, float z, bool sound) {
        respawnPos = new Vector3(x, y, z);

        // Collectable sound
        if (audioManager != null && sound)
            audioManager.PlaySFX("SFX_Collectable");
    }


    // Death animation
    void DeathAnim() {
        if (deathAnimTimer == 0) {
            deathAnimTimer = deathAnimTimerMax;

            // Death sound
            if (audioManager != null)
                audioManager.PlaySFX("SFX_PlayerDeath");
        }
    }


    // Die and respawn
    void DieAndRespawn() {

        // Update death counter
        if (gameManager != null)
            gameManager.PlayerDeath();

        // Respawn sound
        if (audioManager != null)
            audioManager.PlaySFX("SFX_PlayerRespawn");

        // Reset vars
        active          = true;
        moveSpeed       = 0f;
        targetSpeed     = 0f;
        jumping         = false;
        didDoubleJump   = false;
        earlyJumpWindow = 0;
        lateJumpWindow  = 0;
        //numJumps = 0;
        dashing      = false;
        dashInit     = 0;
        dashCooldown = 0;
        gravityScale = fallGrav;

        // Set position
        transform.position = respawnPos;
    }


    void DebugUI() {
        // UI for debug

        UI_Debug.text = "- debug -\n";
        //UI_Debug.text += "ySpeed:" + ySpeed.ToString() + "\n";
        //UI_Debug.text += "gravityScale:" + gravityScale.ToString() + "\n";

        /*
        UI_Debug.text += "target speed\n";
        UI_Debug.text += "  X: " + (moveDirection.x * targetSpeed).ToString() + "\n";
        //UI_Debug.text += "  Y: " + myRB.velocity.y.ToString() + "\n";
        UI_Debug.text += "  Z: " + (moveDirection.z * targetSpeed).ToString() + "\n";
        UI_Debug.text += "speed\n";
        UI_Debug.text += "  X: " + (moveDirection.x * moveSpeed).ToString() + "\n";
        //UI_Debug.text += "  Y: " + myRB.velocity.y.ToString() + "\n";
        UI_Debug.text += "  Z: " + (moveDirection.z * moveSpeed).ToString() + "\n";
        UI_Debug.text += "grav: " + gravityScale.ToString() + "\n";
        UI_Debug.text += "earlyJumpWindow: " + earlyJumpWindow.ToString() + "\n";
        UI_Debug.text += "lateJumpWindow: " + lateJumpWindow.ToString() + "\n";
        UI_Debug.text += "dashInit: " + dashInit.ToString() + "\n";
        UI_Debug.text += "dashCooldown: " + dashCooldown.ToString() + "\n";
        */
    }
}

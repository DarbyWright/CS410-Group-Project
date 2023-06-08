/*
    Any additional code that Ive added to is commented with *** before and after the comment.

    There are sections seperated with --------------- which indicate sections that ive copied
    over from the other player cointroller script.

*/

using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Space(10)]

        [Tooltip("Dash speed of the character in m/s")] // *** Added ***
        public float DashSpeed = 45f;

        [Tooltip("Dash Duration in arbitrary measurments")] // *** Added ***
        public int DashLength = 100;

        [Space(10)]

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Tooltip("Percentage of gravity when glide is used")] // *** Added ***
        public float GlideGravity = 0.5f;

        [Space(10)]

        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass after jumping before being able to double jump.")] // *** Added ***
        public float DoubleJumpTimeout = 40f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _doubleJumpTimeoutDelta; // ***Added***
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private ParticleSystem _particleSystem;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }
// -----------------------------------------------------------------------------------------------
        // *** Added ***

        // Game and audio managers
        GameManager gameManager;
        AudioManager audioManager;

        // Unlockalble abilities
        public bool canDoubleJump = false;
        public bool canDash       = false;
        public bool canGlide      = false;

        // Jumping
        bool didJump       = false;
        bool didDoubleJump = false;

        // Gliding
        bool isGliding = false;
        int glidingDelayTimer = 0;  // Time player must try and glide before sound/effects are made
        int glidingDelayTimerMax = 3 * 60;

        // Dash
        int dashCooldown      = 0;
        int dashCooldownMax   = 3 * 60;
        int dashTimeRemaining = 0;

        // Whether the player can be controlled on not (pausing, death animation, cutscenes, etc)
        public bool active = true;

        // Death, animation time, and respawn location
        int deathAnimTimer    = 0;
        int deathAnimTimerMax = 2 * 60;
        public Vector3 respawnPos;

        public bool dead = false;
        public bool hasKey = false;

// -----------------------------------------------------------------------------------------------

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Start()
        {

            // ------------------------------------------------------------------------
            // *** Get game and audio managers ***
            gameManager  = FindObjectOfType<GameManager>();
            audioManager = FindObjectOfType<AudioManager>();

            // *** Get abilities if the game manager has tracked them as collected ***
            canDoubleJump = gameManager.hasDoubleJump; // *** Added ***
            canDash       = gameManager.hasDash; // *** Added ***
            canGlide      = gameManager.hasGlide; // *** Added ***

            // ------------------------------------------------------------------------

            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _particleSystem = GetComponent<ParticleSystem>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _doubleJumpTimeoutDelta = DoubleJumpTimeout; // *** Added ***
            _fallTimeoutDelta = FallTimeout;

            SetCheckPoint(0, -4, 0, false); // Set default spawn
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            handleTimers(); // *** Added ***

            Move();

            JumpAndGravity();
            GroundedCheck();

        }

        private void LateUpdate()
        {
           // *** if paused disable camera since it is immune to timeScale changes ***
            if(!PauseMenu.Paused)
                CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // Player hits respawn button -- doesn't count as death?
            if (_input.respawn) { 
                _verticalVelocity = 0f;
                _speed = 0f;

                // Update death counter // Uncomment if want to count this as a death
                // if (gameManager != null)
                //     gameManager.PlayerDeath();

                // Reset vars
                active          = true;
                didDoubleJump   = false;
                _speed = 0;
                _verticalVelocity = 0;
                dashCooldown = 0;

                // Set position
                transform.position = respawnPos;
                
                // Respawn sound
                // if (audioManager != null)
                //     audioManager.PlaySFX("SFX_PlayerRespawn");
            }
            // else
            //     _input.respawn = false;

            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            // set target speed based on move speed, sprint speed and if sprint is pressed

            // ----------------------------------------------------------------------------------------
            // *** Dashing currently just increases target speed ***
            if (_input.dash && canDash && dashCooldown <= 0)
            {
                dashCooldown = dashCooldownMax;
                dashTimeRemaining = DashLength;
                _animator.CrossFade("Dash", 0);

                // Dash sound
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_PlayerDash");
            }

            if (dashTimeRemaining > 0)
                targetSpeed = DashSpeed;
            else
                _input.dash = false;

            if (!_input.glide && isGliding) {    // Now that player is grounded, if they were gliding,
                    audioManager.StopSFX("SFX_Jetpack"); // Stop Jetpack sound
                    _particleSystem.Stop(); // Turn off jetpack
                    isGliding = false;
                    glidingDelayTimer = glidingDelayTimerMax;
                }
            // ----------------------------------------------------------------------------------------

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                dead = false; // *** ADDED ***
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                didJump = false;
                didDoubleJump = false; // *** Added ***

                if (isGliding) {    // Now that player is grounded, if they were gliding,
                    audioManager.StopSFX("SFX_Jetpack"); // Stop Jetpack sound
                    _particleSystem.Stop(); // Turn off jetpack
                    isGliding = false;
                    glidingDelayTimer = glidingDelayTimerMax;
                }

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && !didJump)
                {
                    didJump = true;

                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }

                    // reset the jump timeout timer
                    _jumpTimeoutDelta = JumpTimeout;

                    // Jump sound
                    if (audioManager != null)
                        audioManager.PlaySFX("SFX_PlayerJump");

                }
                // If you're grounded and you didn't jump, reset double jump timeout delta
                else
                    _doubleJumpTimeoutDelta = DoubleJumpTimeout; // *** Added ***
            }
            else
            {
                // -------------------------------------------------------------------------------------
                // *** If the player is not grounded, check for double Jump ***
                if (_input.jump && canDoubleJump && _doubleJumpTimeoutDelta <= 0.0f && !didDoubleJump)
                {
                    didDoubleJump = true;

                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        print("CROSSFADE");
                        _animator.CrossFade("DoubleJump", 0);
                    }

                    // Double jump sound
                    if (audioManager != null)
                        audioManager.PlaySFX("SFX_PlayerDoubleJump");
                }
                // -------------------------------------------------------------------------------------

                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false ;

                // -----------------------------------------------------------------------------
                // *** Once we're in the air, start decrementing double jump fallout delta. ***
                if (_doubleJumpTimeoutDelta >= 0.0f)
                {
                    _doubleJumpTimeoutDelta -= Time.deltaTime;
                }
                // -----------------------------------------------------------------------------
            }

            // --------------------------------------------------------------------------------------------------------------
            // *** Super simple Glide. Just reducing gravity but only if the playuer is moving down ***
            if (_input.glide && canGlide)
            {

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity += Gravity * GlideGravity * Time.deltaTime;
                else
                    _verticalVelocity += Gravity * Time.deltaTime;

                // Use a delay timer to wait to see if user is trying to glide for a few frames before making sound/particles
                if (glidingDelayTimer <= 0) {
                    // Check if characacter is already gliding -- to prevent starting audio/particle clips every frame
                    if (!isGliding) {
                        _particleSystem.Play();
                        audioManager.PlaySFX("SFX_Jetpack");
                    }
                    isGliding = true;
                }

                if (!isGliding) {
                    glidingDelayTimer--;
                }
                else {
                    glidingDelayTimer = glidingDelayTimerMax;
                }
                
                
            }
            // ---------------------------------------------------------------------------------------------------------------

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            else if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
// -------------------------------------------------------------------------------------------------------------------------------------------------
        // Everything above was from the ThirdPersonController package (excewpt sections with dashed lines or lines with *** Added ***).
        // Everything below was copied over from our other original player controller script and adapted to work in this one.

        void handleTimers() {

            // Time until can dash again
            if (dashCooldown > 0) dashCooldown--;
            if (dashTimeRemaining > 0) dashTimeRemaining--;

            // Death animation / cutscene
            if (deathAnimTimer > 0) deathAnimTimer--;

            // At the end of the death animation, die and respawn
            if (deathAnimTimer == 1)
                DieAndRespawn();
        }

        // Triggers
        void OnTriggerEnter(Collider other) {

            // Music trigger
            if (other.gameObject.CompareTag("MusicTrigger")) {
                // TODO ???
            }

            if (other.gameObject.CompareTag("TempLvl2Trigger"))
            {
                SetCheckPoint(other.transform.position.x, other.transform.position.y, other.transform.position.z, false);
            }

            // DoubleJump item
            if (other.gameObject.CompareTag("ItemDoubleJump")) {
                gameManager.GotDoubleJump();
                canDoubleJump = true;
                SetCheckPoint(other.transform.position.x, other.transform.position.y, other.transform.position.z, false);
                Destroy(other.gameObject);

                // Item Jingle
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_SpecialEvent");
            }

            // Dash item
            if (other.gameObject.CompareTag("ItemDash")) {
                gameManager.GotDash();
                canDash = true;
                SetCheckPoint(other.transform.position.x, other.transform.position.y, other.transform.position.z, false);
                Destroy(other.gameObject);

                // Item Jingle
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_SpecialEvent");
            }

            // Glide item
            if (other.gameObject.CompareTag("ItemGlide")) {
                gameManager.GotGlide();
                canGlide = true;
                SetCheckPoint(other.transform.position.x, other.transform.position.y, other.transform.position.z, false);
                Destroy(other.gameObject);

                // Item Jingle
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_SpecialEvent");
            }

            // Checkpoints
            if (other.gameObject.CompareTag("Checkpoint")) {
                float x = other.gameObject.transform.position.x;
                float y = other.gameObject.transform.position.y;
                float z = other.gameObject.transform.position.z;

                // Only set checkpoint if this is a differnet one (for sound or animation if we want)
                if (!(x == respawnPos.x && y == respawnPos.y && z == respawnPos.z))
                    SetCheckPoint(x, y, z, true);
                Destroy(other.gameObject);
            }

            // Geysers
            if (other.gameObject.CompareTag("Geyser")) {
                _verticalVelocity = 50f;
                if (audioManager != null) {
                    audioManager.PlaySFX("SFX_Geyser_3");
                }
                ParticleSystem geyser_PS = other.GetComponent<ParticleSystem>(); 
                geyser_PS.Play();
                Debug.Log("Geyser activated");
            }

            // Death and Respawning
            if (other.gameObject.CompareTag("Enemy"))
            {
                // DeathAnim();
                dead = true;
                DieAndRespawn();
            }

            if (other.gameObject.CompareTag("OutOfBounds")) 
            {
                // DeathAnim();
                dead = true;
                DieAndRespawn();
            }

            if (other.gameObject.CompareTag("Key")) 
            {
                hasKey = true;
                Destroy(other.gameObject);
                // Item Jingle
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_SpecialEvent");
                Debug.Log("Key Aquired");
            }

            if (other.gameObject.CompareTag("LockedGate")) 
            {
                if (hasKey) {
                    Destroy(other.gameObject);

                    // Door unlock sound
                    if (audioManager != null)
                        audioManager.PlaySFX("SFX_UnlockDoor");
                    Debug.Log("Door Unlocked");
                }
                
            }

            // LEVEL COMPLETEIONS                               //////////////// TODO
            // Complete Level 1
            if (other.gameObject.CompareTag("Complete_Level1")) 
            {
                gameManager.finishedCave = true;
                // Item Jingle
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_LevelComplete");
            }

            // Complete Level 2
            if (other.gameObject.CompareTag("Complete_Level2")) 
            {
                gameManager.finishedMine = true;
                // Item Jingle
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_LevelComplete");
            }

            // Complete Level 3
            if (other.gameObject.CompareTag("Complete_Level3")) 
            {
                gameManager.finishedCanyon = true;
                // Item Jingle
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_LevelComplete");
            }

            // Complete the game
            if (other.gameObject.CompareTag("Complete_Level4")) 
            {
                // TODO
                Destroy(other.gameObject);
                // Item Jingle
                if (audioManager != null)
                    audioManager.PlaySFX("SFX_LevelComplete");
                Debug.Log("Completed Level 4");
            }
        }                                                    ////////////////////////////////

        // Collide with objects
        // void OnControllerColliderHit(ControllerColliderHit collision) {

        //     // If a projectile hits player, die
        //     if (CanDieFromCollision(collision)){
        //         if(!dead)
        //         {
        //             dead = true;
        //             DieAndRespawn();   
        //         }
        //     }
        // }


        // bool CanDieFromCollision(ControllerColliderHit collision)
        // {
        //     return collision.gameObject.CompareTag("Enemy") ||
        //            collision.gameObject.CompareTag("OutOfBounds");
        // }


        // Update the player's respawn position
        void SetCheckPoint(float x, float y, float z, bool sound) {
            respawnPos = new Vector3(x, y, z);

            // Update spawnpoint
            if (gameManager != null) {
                gameManager.SetSpawn(respawnPos);
            }

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
        public void DieAndRespawn() {
            _verticalVelocity = 0f;
            _speed = 0f;

            // Update death counter
            if (gameManager != null)
                gameManager.PlayerDeath();

            // Reset vars
            active          = true;
            didDoubleJump   = false;
            _speed = 0;
            _verticalVelocity = 0;
            dashCooldown = 0;

            // Set position
            transform.position = respawnPos;

            // Respawn sound
            if (audioManager != null)
                audioManager.PlaySFX("SFX_PlayerRespawn");

            dead = false;
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 8.5f;
    public float rotationSpeed = 10.0f;
    public float jumpHeight = 1.6f;
    public float gravity = -18.0f;

    [Header("Input Action References (Optional / Fallback to Keyboard)")]
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private InputActionReference interactControl;

    [Header("Interaction Settings")]
    public float interactRadius = 2.5f;
    public LayerMask interactableLayers = ~0;

    [Header("Respawn Safety")]
    public float fallThreshold = -12.0f;
    private Vector3 safeSpawnPoint;
    private Vector3 levelStartSpawnPoint;
    private bool isRespawning = false;
    private bool isInvulnerable = false;
    private Vector3 knockbackVelocity = Vector3.zero;

    // Component references
    private CharacterController controller;
    private Transform cameraTransform;
    private Animator animator;

    // Movement state
    private Vector3 velocity;
    private bool isGrounded;
    private float stepTimer = 0f;
    private bool isSprinting = false;
    private bool isVictory = false;

    // Current interactable in reach
    private IInteractable currentInteractable;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        safeSpawnPoint = transform.position;
    }

    private void OnEnable()
    {
        try { movementControl?.action?.Enable(); } catch { }
        try { jumpControl?.action?.Enable(); } catch { }
        try { interactControl?.action?.Enable(); } catch { }
    }

    private void OnDisable()
    {
        try { movementControl?.action?.Disable(); } catch { }
        try { jumpControl?.action?.Disable(); } catch { }
        try { interactControl?.action?.Disable(); } catch { }
    }

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Initialize safe spawn points
        levelStartSpawnPoint = transform.position;
        safeSpawnPoint = transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused)
            return;

        if (isRespawning)
            return;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Safety respawn if fallen off edge
        if (transform.position.y < fallThreshold)
        {
            OnFallIntoAbyss();
            return;
        }

        CheckGrounded();
        HandleMovement();
        HandleInteraction();
        UpdateAnimations();
    }

    private void CheckGrounded()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Slight downward pressure to stick to slopes
        }

        // Only update safe ground position when on solid, dry, elevated ground
        if (isGrounded && Mathf.Abs(velocity.y) < 1f && transform.position.y > 0.4f)
        {
            safeSpawnPoint = transform.position;
        }
    }

    private void HandleMovement()
    {
        // Decay knockback
        if (knockbackVelocity.sqrMagnitude > 0.01f)
        {
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, 8f * Time.deltaTime);
        }
        else
        {
            knockbackVelocity = Vector3.zero;
        }

        Vector2 input = Vector2.zero;

        // Try reading from Input Action Reference first
        if (movementControl != null && movementControl.action != null && movementControl.action.enabled)
        {
            input = movementControl.action.ReadValue<Vector2>();
        }

        // Fallback to standard input (WASD / Arrow Keys)
        if (input.sqrMagnitude < 0.01f)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            input = new Vector2(h, v);
            if (input.sqrMagnitude > 1f) input.Normalize();
        }

        // Sprint input
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Calculate world move direction relative to camera
        Vector3 moveDirection = Vector3.zero;
        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
            Vector3 camRight = cameraTransform != null ? cameraTransform.right : Vector3.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = (camForward * input.y + camRight * input.x).normalized;

            // Rotate towards move direction
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

            // Footstep sounds
            stepTimer += Time.deltaTime;
            float stepInterval = isSprinting ? 0.3f : 0.45f;
            if (stepTimer >= stepInterval && isGrounded)
            {
                stepTimer = 0f;
                SoundManager.Instance?.PlayFootstep();
            }
        }
        else
        {
            stepTimer = 0f;
        }

        // Jump
        bool jumpRequested = false;
        if (jumpControl != null && jumpControl.action != null && jumpControl.action.triggered)
        {
            jumpRequested = true;
        }
        if (!jumpRequested && Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }

        if (jumpRequested && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            SoundManager.Instance?.PlayJumpSound();
            PlayClipIfPossible("JumpFull_Normal_InPlace_SwordAndShield");
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine horizontal movement, vertical velocity, and knockback
        Vector3 totalMotion = (moveDirection * currentSpeed + Vector3.up * velocity.y + knockbackVelocity) * Time.deltaTime;
        controller.Move(totalMotion);
    }

    private void HandleInteraction()
    {
        // Find nearest interactable
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactableLayers, QueryTriggerInteraction.Collide);
        IInteractable nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<IInteractable>() ?? hit.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract(this))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = interactable;
                }
            }
        }

        currentInteractable = nearest;

        // Check interaction button
        bool interactPressed = false;
        if (interactControl != null && interactControl.action != null && interactControl.action.triggered)
        {
            interactPressed = true;
        }
        if (!interactPressed && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)))
        {
            interactPressed = true;
        }

        if (interactPressed && currentInteractable != null && currentInteractable.CanInteract(this))
        {
            currentInteractable.Interact(this);
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;
        if (isVictory) return;

        Vector3 horizontalVel = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        float currentMoveMagnitude = horizontalVel.magnitude;

        // Set animator float if parameter exists
        try
        {
            animator.SetFloat("Speed", currentMoveMagnitude);
            animator.SetBool("IsGrounded", isGrounded);
        }
        catch { }

        // Directly trigger animation states if using default animation controller
        if (!isGrounded)
        {
            PlayClipIfPossible("JumpFull_Normal_InPlace_SwordAndShield");
        }
        else if (currentMoveMagnitude > 0.3f)
        {
            if (isSprinting && currentMoveMagnitude > 5.5f)
            {
                PlayClipIfPossible("SprintFWD_Battle_InPlace_SwordAndShield");
            }
            else
            {
                PlayClipIfPossible("MoveFWD_Normal_InPlace_SwordAndShield");
            }
        }
        else
        {
            PlayClipIfPossible("Idle_Normal_SwordAndShield");
        }
    }

    private void PlayClipIfPossible(string stateName)
    {
        if (animator == null || !animator.isActiveAndEnabled) return;
        if (animator.HasState(0, Animator.StringToHash(stateName)))
        {
            var currentInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!currentInfo.IsName(stateName))
            {
                animator.CrossFade(stateName, 0.15f);
            }
        }
    }

    public void TriggerVictoryAnimation()
    {
        isVictory = true;
        if (animator != null)
        {
            PlayClipIfPossible("Victory_Battle_SwordAndShield");
        }
    }

    public void OnFallIntoWater(Vector3 waterSurfacePos)
    {
        if (isInvulnerable || isRespawning) return;
        StartCoroutine(DrownInWaterRoutine(waterSurfacePos));
    }

    private System.Collections.IEnumerator DrownInWaterRoutine(Vector3 waterSurfacePos)
    {
        isRespawning = true;
        velocity = Vector3.zero;
        knockbackVelocity = Vector3.zero;

        // 1. Splash audio and visual
        SoundManager.Instance?.PlayWaterSplash();
        CreateWaterSplashEffect(waterSurfacePos);

        // 2. Hurt and loss of life
        GameManager.Instance?.LoseLife("The Captain fell into the deep ocean!");

        // If game over, stop here
        if (GameManager.Instance != null && GameManager.Instance.currentLives <= 0)
        {
            isRespawning = false;
            yield break;
        }

        // 3. Brief splash delay then clean teleport back to island start
        yield return new WaitForSeconds(0.8f);

        RespawnToSafeCamp();

        // 4. Temporary invulnerability so no instant re-trigger
        isInvulnerable = true;
        isRespawning = false;
        yield return new WaitForSeconds(1.5f);
        isInvulnerable = false;
    }

    public void TakeDamage(int damage, Vector3 hazardPosition)
    {
        if (isInvulnerable || isRespawning) return;

        // Knockback impulse
        Vector3 pushDir = (transform.position - hazardPosition).normalized;
        pushDir.y = 0.4f;
        knockbackVelocity = pushDir * 9.0f;

        GameManager.Instance?.LoseLife("Struck by lethal ancient spikes!");

        StartCoroutine(DamageFlashRoutine());
    }

    private System.Collections.IEnumerator DamageFlashRoutine()
    {
        isInvulnerable = true;
        // Blink renderers
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < 4; i++)
        {
            foreach (var r in rends) if (r != null) r.enabled = false;
            yield return new WaitForSeconds(0.1f);
            foreach (var r in rends) if (r != null) r.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        isInvulnerable = false;
    }

    public void OnFallIntoAbyss()
    {
        if (isRespawning) return;
        StartCoroutine(AbyssFallRoutine());
    }

    private System.Collections.IEnumerator AbyssFallRoutine()
    {
        isRespawning = true;
        velocity = Vector3.zero;
        knockbackVelocity = Vector3.zero;

        SoundManager.Instance?.PlayHurt();
        GameManager.Instance?.LoseLife("Fell into the bottomless abyss!");

        if (GameManager.Instance != null && GameManager.Instance.currentLives <= 0)
        {
            isRespawning = false;
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
        RespawnToSafeCamp();

        isInvulnerable = true;
        isRespawning = false;
        yield return new WaitForSeconds(1.2f);
        isInvulnerable = false;
    }

    public void RespawnToSafeCamp()
    {
        controller.enabled = false;
        // Prefer safeSpawnPoint if on high ground, otherwise fallback to level start
        Vector3 targetPos = (safeSpawnPoint.y > 0.4f) ? safeSpawnPoint : levelStartSpawnPoint;
        transform.position = targetPos + Vector3.up * 0.5f;
        velocity = Vector3.zero;
        knockbackVelocity = Vector3.zero;
        controller.enabled = true;
    }

    private void CreateWaterSplashEffect(Vector3 splashPos)
    {
        GameObject splashObj = new GameObject("WaterSplashFX");
        splashObj.transform.position = new Vector3(transform.position.x, splashPos.y, transform.position.z);

        // White foam ring
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "FoamRing";
        ring.transform.SetParent(splashObj.transform, false);
        ring.transform.localScale = new Vector3(2.5f, 0.05f, 2.5f);
        var r = ring.GetComponent<Renderer>();
        r.material = ProceduralProps.GetGlowingMaterial(Color.white, "SplashFoam");
        Destroy(ring.GetComponent<Collider>());

        // Point light splash flash
        var l = splashObj.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = Color.cyan;
        l.range = 5f;
        l.intensity = 3f;

        Destroy(splashObj, 0.8f);
    }

    public IInteractable GetCurrentInteractable() => currentInteractable;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}

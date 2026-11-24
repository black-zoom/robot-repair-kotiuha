using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static readonly int LookXHash = Animator.StringToHash("Look X");
    private static readonly int LookYHash = Animator.StringToHash("Look Y");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int HitTriggerHash = Animator.StringToHash("Hit");
    private static readonly int LaunchTriggerHash = Animator.StringToHash("Launch");

    private const float FootstepMoveThreshold = 0.1f;
    private const float NpcRaycastDistance = 1.5f;
    private const float NpcRayOriginOffsetY = 0.2f;
    private const float NpcRaySideOffset = 0.2f;
    private const float ProjectileSpawnOffsetY = 0.5f;
    private const float ProjectileForce = 300f;

    [Header("Input")]
    public InputAction MoveAction;
    public InputAction LaunchAction;
    public InputAction DashAction;
    public InputAction TalkAction;

    [Header("Movement")]
    public float speed = 3.0f;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public float launchCooldown = 0.5f;

    [Header("Health")]
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    [Header("Audio")]
    public AudioSource footstepAudioSource;
    public AudioClip hitSound;
    public AudioClip launchSound;
    public float footstepDelay = 0.25f;

    public int health => currentHealth;

    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private DashSystem dashSystem;
    private AudioSource audioSource;

    private Vector2 move;
    private Vector2 moveDirection = Vector2.right;
    private float baseSpeed;

    private int currentHealth;
    private bool isInvincible;
    private float damageCooldown;
    private float launchCooldownTimer;
    private float footstepTimer;

    private int npcLayerMask;
    private NonPlayerCharacter lastNonPlayerCharacter;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dashSystem = GetComponent<DashSystem>();
        audioSource = GetComponent<AudioSource>();

        npcLayerMask = LayerMask.GetMask("NPC");
    }

    void OnEnable()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        DashAction.Enable();
        TalkAction.Enable();
    }

    void OnDisable()
    {
        MoveAction.Disable();
        LaunchAction.Disable();
        DashAction.Disable();
        TalkAction.Disable();
    }

    void Start()
    {
        baseSpeed = speed;
        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleMovementInput();
        UpdateAnimator();
        HandleFootsteps();
        UpdateTimers();
        HandleCombatInput();
        HandleNPCInteraction();
        HandleDashInput();
    }

    void FixedUpdate()
    {
        Vector2 movement = (dashSystem != null && dashSystem.isDashing)
            ? dashSystem.dashDirection * dashSystem.dashSpeed
            : move * speed;

        Vector2 position = rigidbody2d.position + movement * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }

            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger(HitTriggerHash);
            PlaySound(hitSound);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void HandleMovementInput()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection = move.normalized;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(LookXHash, moveDirection.x);
        animator.SetFloat(LookYHash, moveDirection.y);
        animator.SetFloat(SpeedHash, move.magnitude);
    }

    private void HandleFootsteps()
    {
        bool isMoving = move.magnitude > FootstepMoveThreshold;
        bool isDashing = dashSystem != null && dashSystem.isDashing;

        if (isMoving && !isDashing)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                float speedMultiplier = (speed > 0f && baseSpeed > 0f) ? (speed / baseSpeed) : 1f;
                footstepTimer = footstepDelay / speedMultiplier;
            }
        }
        else
        {
            footstepTimer = 0f;
            if (footstepAudioSource != null && footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }
    }

    private void UpdateTimers()
    {
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown <= 0f)
            {
                isInvincible = false;
            }
        }

        if (launchCooldownTimer > 0f)
        {
            launchCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleCombatInput()
    {
        if (LaunchAction.WasPressedThisFrame() && launchCooldownTimer <= 0f)
        {
            Launch();
            launchCooldownTimer = launchCooldown;
        }
    }

    private void HandleDashInput()
    {
        if (dashSystem != null && DashAction.WasPressedThisFrame())
        {
            dashSystem.StartDash(move);
        }
    }

    private void HandleNPCInteraction()
    {
        NonPlayerCharacter npc = FindNearbyNPC();

        if (lastNonPlayerCharacter != null && lastNonPlayerCharacter != npc)
        {
            lastNonPlayerCharacter.dialogueBubble.SetActive(false);

            if (UIHandler.instance != null)
            {
                UIHandler.instance.StartHideDialogueTimer();
            }
        }

        if (npc != null)
        {
            npc.dialogueBubble.SetActive(true);
            if (TalkAction.WasPressedThisFrame() && UIHandler.instance != null)
            {
                UIHandler.instance.DisplayDialogue(npc.dialogueText);
            }
        }

        lastNonPlayerCharacter = npc;
    }

    private NonPlayerCharacter FindNearbyNPC()
    {
        Vector2 rayOrigin = rigidbody2d.position + Vector2.up * NpcRayOriginOffsetY;
        Vector2 sideOffset = Vector2.Perpendicular(moveDirection) * NpcRaySideOffset;

        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin + sideOffset, moveDirection, NpcRaycastDistance, npcLayerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin - sideOffset, moveDirection, NpcRaycastDistance, npcLayerMask);

        return GetNPC(hit1) ?? GetNPC(hit2);
    }

    private NonPlayerCharacter GetNPC(RaycastHit2D hit)
    {
        return hit.collider != null
            ? hit.collider.GetComponent<NonPlayerCharacter>()
            : null;
    }

    private void Launch()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            rigidbody2d.position + Vector2.up * ProjectileSpawnOffsetY,
            Quaternion.identity
        );

        if (projectileObject.TryGetComponent(out Projectile projectile))
        {
            projectile.Launch(moveDirection, ProjectileForce);
        }

        animator.SetTrigger(LaunchTriggerHash);
        PlaySound(launchSound);
    }

    private void PlayFootstep()
    {
        if (footstepAudioSource != null)
        {
            footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);
            footstepAudioSource.Play();
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    Vector2 moveDirection = new Vector2(1, 0);
    Animator animator;
    public GameObject projectilePrefab;
    public InputAction DashAction;
    DashSystem dashSystem;

    public float speed = 3.0f;

    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    public float launchCooldown = 0.5f;
    float launchCooldownTimer;

    public InputAction TalkAction;
    private NonPlayerCharacter lastNonPlayerCharacter;

    AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip launchSound;

    void Start()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        DashAction.Enable();

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dashSystem = GetComponent<DashSystem>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;

        TalkAction.Enable();
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f)
            || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;

            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (launchCooldownTimer > 0)
        {
            launchCooldownTimer -= Time.deltaTime;
        }

        if (LaunchAction.WasPressedThisFrame() && launchCooldownTimer <= 0)
        {
            Launch();
            launchCooldownTimer = launchCooldown;
        }
        
        Vector2 rayOrigin = rigidbody2d.position + Vector2.up * 0.2f;
        Vector2 sideOffset = Vector2.Perpendicular(moveDirection) * 0.2f;

        RaycastHit2D hit1 = Physics2D.Raycast(
            rayOrigin + sideOffset,
            moveDirection,
            1.5f,
            LayerMask.GetMask("NPC")
        );

        RaycastHit2D hit2 = Physics2D.Raycast(
            rayOrigin - sideOffset,
            moveDirection,
            1.5f,
            LayerMask.GetMask("NPC")
        );

        NonPlayerCharacter npc = GetNPC(hit1) ?? GetNPC(hit2);

        if (npc != null)
        {
            npc.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = npc;
            FindFriend(npc);
        }
        else if (lastNonPlayerCharacter != null)
        {
            lastNonPlayerCharacter.dialogueBubble.SetActive(false);
            lastNonPlayerCharacter = null;
        }

        if (DashAction.WasPressedThisFrame())
        {
            dashSystem.StartDash(move);
        }
    }
    
    void FixedUpdate()
    {
        Vector2 movement;

        if (dashSystem != null && dashSystem.isDashing)
        {
            movement = dashSystem.dashDirection * dashSystem.dashSpeed;
        }
        else
        {
            movement = move * speed;
        }

        Vector2 position = rigidbody2d.position
            + movement * Time.fixedDeltaTime;

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
            animator.SetTrigger("Hit");
            PlaySound(hitSound);
        }

        currentHealth = Mathf.Clamp(
            currentHealth + amount,
            0,
            maxHealth
        );

        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f,
            Quaternion.identity
        );

        Projectile projectile =projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);

        animator.SetTrigger("Launch");
        PlaySound(launchSound);
    }

    NonPlayerCharacter GetNPC(RaycastHit2D hit)
    {
        return hit.collider != null
            ? hit.collider.GetComponent<NonPlayerCharacter>()
            : null;
    }

    void FindFriend(NonPlayerCharacter npc)
    {
        if (TalkAction.WasPressedThisFrame())
        {
            UIHandler.instance.DisplayDialogue(npc.dialogueText);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}


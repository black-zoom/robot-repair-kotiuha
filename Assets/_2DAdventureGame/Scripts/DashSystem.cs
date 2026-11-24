using UnityEngine;

public class DashSystem : MonoBehaviour
{
    private const float MinDashPitch = 0.95f;
    private const float MaxDashPitch = 1.05f;

    [Header("Dash")]
    public float dashSpeed = 9f;
    public float dashDuration = 0.1f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaCost = 50f;
    public float recoveryRate = 20f;
    public float recoveryDelay = 0.5f;
    public float currentStamina;

    [Header("Audio")]
    public AudioClip dashSound;

    public bool isDashing { get; private set; }
    public Vector2 dashDirection { get; private set; }
    public float StaminaNormalized => maxStamina > 0f
        ? Mathf.Clamp01(currentStamina / maxStamina)
        : 0f;

    private AudioSource audioSource;
    private float dashTimer;
    private float recoveryTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (isDashing)
        {
            UpdateDash();
            return;
        }

        UpdateStaminaRecovery();
    }

    public void StartDash(Vector2 direction)
    {
        if (isDashing || currentStamina < staminaCost || direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        isDashing = true;
        dashDirection = direction.normalized;
        currentStamina = Mathf.Max(0f, currentStamina - staminaCost);
        dashTimer = dashDuration;

        PlayDashSound();
    }

    private void UpdateDash()
    {
        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f)
        {
            isDashing = false;
            recoveryTimer = recoveryDelay;
        }
    }

    private void UpdateStaminaRecovery()
    {
        if (recoveryTimer > 0f)
        {
            recoveryTimer -= Time.deltaTime;
            return;
        }

        if (currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(currentStamina + recoveryRate * Time.deltaTime, maxStamina);
        }
    }

    private void PlayDashSound()
    {
        if (audioSource == null || dashSound == null)
        {
            return;
        }

        audioSource.pitch = Random.Range(MinDashPitch, MaxDashPitch);
        audioSource.PlayOneShot(dashSound);
    }
}

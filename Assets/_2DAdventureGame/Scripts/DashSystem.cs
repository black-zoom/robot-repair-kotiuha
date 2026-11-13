using UnityEngine;

public class DashSystem : MonoBehaviour
{
    public float dashSpeed = 9f;
    public float dashDuration = 0.1f;

    public float maxStamina = 100f;
    public float staminaCost = 50f;
    public float recoveryRate = 20f;
    public float recoveryDelay = 0.5f;

    public float currentStamina;
    public bool isDashing { get; private set; }
    public Vector2 dashDirection { get; private set; }

    float dashTimer;
    float recoveryTimer;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
                recoveryTimer = recoveryDelay;
            }

            return;
        }

        if (recoveryTimer > 0)
        {
            recoveryTimer -= Time.deltaTime;
            return;
        }

        currentStamina = Mathf.Min(
            currentStamina + recoveryRate * Time.deltaTime,
            maxStamina
        );
    }
    
    public void StartDash(Vector2 direction)
    {
        if (isDashing || currentStamina < staminaCost)
            return;

        if (direction == Vector2.zero)
            return;

        isDashing = true;
        dashDirection = direction.normalized;

        currentStamina -= staminaCost;
        dashTimer = dashDuration;
    }
}

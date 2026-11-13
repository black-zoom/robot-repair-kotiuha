using UnityEngine;
using UnityEngine.UIElements;

public class UIStamina : MonoBehaviour
{
    DashSystem dashSystem;

    VisualElement staminaContainer;
    VisualElement staminaFill;

    float hideTimer;
    float displayedStamina = 100f;

    public float hideDelay = 1f;
    public float fadeSpeed = 4f;
    public float fillSpeed = 150f;

    void Start()
    {
        dashSystem = FindFirstObjectByType<DashSystem>();

        var root = GetComponent<UIDocument>().rootVisualElement;

        staminaContainer = root.Q<VisualElement>("StaminaBarContainer");
        staminaFill = root.Q<VisualElement>("StaminaBarFill");

        if (dashSystem == null || staminaContainer == null ||
            staminaFill == null)
        {
            Debug.LogError("UIStamina: Missing references!");
            enabled = false;
            return;
        }

        displayedStamina = dashSystem.currentStamina;
        staminaContainer.style.opacity = 0f;
    }

    void Update()
    {
        float stamina = dashSystem.currentStamina;
        float maxStamina = dashSystem.maxStamina;

        float targetStamina = maxStamina > 0f
            ? Mathf.Clamp01(stamina / maxStamina) * 100f
            : 0f;

        displayedStamina = Mathf.MoveTowards(
            displayedStamina,
            targetStamina,
            fillSpeed * Time.deltaTime
        );

        staminaFill.style.width = Length.Percent(displayedStamina);

        if (targetStamina < 100f)
        {
            hideTimer = hideDelay;
        }
        else
        {
            hideTimer -= Time.deltaTime;
        }

        float targetOpacity = hideTimer > 0f ? 1f : 0f;

        staminaContainer.style.opacity = Mathf.MoveTowards(
            staminaContainer.resolvedStyle.opacity,
            targetOpacity,
            fadeSpeed * Time.deltaTime
        );
    }
}

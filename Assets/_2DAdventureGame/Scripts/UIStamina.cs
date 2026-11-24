using UnityEngine;
using UnityEngine.UIElements;

public class UIStamina : MonoBehaviour
{
    private const string StaminaContainerName = "StaminaBarContainer";
    private const string StaminaFillName = "StaminaBarFill";
    private const float FullStaminaPercent = 100f;

    [Header("Display")]
    public float hideDelay = 1f;
    public float fadeSpeed = 4f;
    public float fillSpeed = 150f;

    private DashSystem dashSystem;
    private VisualElement staminaContainer;
    private VisualElement staminaFill;

    private float hideTimer;
    private float displayedStamina = FullStaminaPercent;
    private float currentOpacity;

    private void Start()
    {
        dashSystem = FindFirstObjectByType<DashSystem>();

        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null || uiDocument.rootVisualElement == null)
        {
            Debug.LogError("UIStamina: Missing UIDocument!");
            enabled = false;
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        staminaContainer = root.Q<VisualElement>(StaminaContainerName);
        staminaFill = root.Q<VisualElement>(StaminaFillName);

        if (dashSystem == null || staminaContainer == null || staminaFill == null)
        {
            Debug.LogError("UIStamina: Missing references!");
            enabled = false;
            return;
        }

        displayedStamina = dashSystem.StaminaNormalized * FullStaminaPercent;
        staminaFill.style.width = Length.Percent(displayedStamina);

        currentOpacity = 0f;
        staminaContainer.style.opacity = currentOpacity;
    }

    private void Update()
    {
        float targetStamina = dashSystem.StaminaNormalized * FullStaminaPercent;

        UpdateStaminaFill(targetStamina);
        UpdateVisibility(targetStamina);
    }

    private void UpdateStaminaFill(float targetStamina)
    {
        if (Mathf.Approximately(displayedStamina, targetStamina))
        {
            return;
        }

        displayedStamina = Mathf.MoveTowards(displayedStamina, targetStamina, fillSpeed * Time.deltaTime);
        staminaFill.style.width = Length.Percent(displayedStamina);
    }

    private void UpdateVisibility(float targetStamina)
    {
        if (targetStamina < FullStaminaPercent)
        {
            hideTimer = hideDelay;
        }
        else if (hideTimer > 0f)
        {
            hideTimer = Mathf.Max(0f, hideTimer - Time.deltaTime);
        }

        float targetOpacity = hideTimer > 0f ? 1f : 0f;
        if (Mathf.Approximately(currentOpacity, targetOpacity))
        {
            return;
        }

        currentOpacity = Mathf.MoveTowards(currentOpacity, targetOpacity, fadeSpeed * Time.deltaTime);
        staminaContainer.style.opacity = currentOpacity;
    }
}

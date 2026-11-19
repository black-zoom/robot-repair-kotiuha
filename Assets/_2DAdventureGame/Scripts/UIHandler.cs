using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }
    
    public float displayTime = 4.0f;
    public float fadeInDuration = 0.5f;

    private VisualElement m_Healthbar;
    private VisualElement m_NonPlayerDialogue;
    private float m_TimerDisplay;
    private Label m_DialogueLabel;

    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;
    private VisualElement m_FadeOverlay;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();

        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);

        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimerDisplay = -1.0f;

        m_DialogueLabel = m_NonPlayerDialogue.Q<Label>("DialogueText");

        m_LoseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
        m_WinScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");
        m_FadeOverlay = uiDocument.rootVisualElement.Q<VisualElement>("FadeOverlay");

        if (m_FadeOverlay != null)
        {
            m_FadeOverlay.style.opacity = 1f;
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        yield return null;
        yield return null;

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            m_FadeOverlay.style.opacity = 1f - Mathf.Clamp01(timer / fadeInDuration);
            yield return null;
        }

        m_FadeOverlay.style.opacity = 0f;
    }

    private void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void DisplayDialogue(string dialogue)
    {
        m_DialogueLabel.text = dialogue;
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    public void DisplayWinScreen()
    {
        m_WinScreen.style.opacity = 1.0f;
    }

    public void DisplayLoseScreen()
    {
        m_LoseScreen.style.opacity = 1.0f;
    }
}

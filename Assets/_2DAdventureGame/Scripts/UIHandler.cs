using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private const string HealthBarName = "HealthBar";
    private const string NpcDialogueName = "NPCDialogue";
    private const string DialogueTextName = "DialogueText";
    private const string WinScreenName = "WinScreenContainer";
    private const string LoseScreenName = "LoseScreenContainer";
    private const string FadeOverlayName = "FadeOverlay";
    private const float MaxFadeDeltaTime = 0.05f;

    public static UIHandler instance { get; private set; }

    [Header("Dialogue")]
    public float dialogueHideDelay = 1.0f;

    [Header("Transitions")]
    public float fadeInDuration = 0.5f;

    private VisualElement m_Healthbar;
    private VisualElement m_NonPlayerDialogue;
    private Label m_DialogueLabel;
    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;
    private VisualElement m_FadeOverlay;

    private float m_HideTimer;
    private bool m_IsDialogueOpen;
    private bool m_IsClosingDialogue;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        InitializeUI();
        StartFadeIn();
    }

    private void Update()
    {
        UpdateDialogueTimer();
    }

    public void DisplayDialogue(string dialogue)
    {
        if (m_NonPlayerDialogue == null || m_DialogueLabel == null)
        {
            return;
        }

        m_DialogueLabel.text = dialogue;
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_IsDialogueOpen = true;
        m_IsClosingDialogue = false;
    }

    public void StartHideDialogueTimer()
    {
        if (!m_IsDialogueOpen)
        {
            return;
        }

        m_IsClosingDialogue = true;
        m_HideTimer = dialogueHideDelay;
    }

    public void SetHealthValue(float percentage)
    {
        if (m_Healthbar != null)
        {
            m_Healthbar.style.width = Length.Percent(100f * Mathf.Clamp01(percentage));
        }
    }

    public void DisplayWinScreen()
    {
        if (m_WinScreen != null)
        {
            m_WinScreen.style.opacity = 1.0f;
        }
    }

    public void DisplayLoseScreen()
    {
        if (m_LoseScreen != null)
        {
            m_LoseScreen.style.opacity = 1.0f;
        }
    }

    private void InitializeUI()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null || uiDocument.rootVisualElement == null)
        {
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;

        m_Healthbar = root.Q<VisualElement>(HealthBarName);
        SetHealthValue(1.0f);

        m_NonPlayerDialogue = root.Q<VisualElement>(NpcDialogueName);
        if (m_NonPlayerDialogue != null)
        {
            m_DialogueLabel = m_NonPlayerDialogue.Q<Label>(DialogueTextName);
            HideDialogue();
        }

        m_LoseScreen = root.Q<VisualElement>(LoseScreenName);
        m_WinScreen = root.Q<VisualElement>(WinScreenName);
        m_FadeOverlay = root.Q<VisualElement>(FadeOverlayName);
    }

    private void UpdateDialogueTimer()
    {
        if (!m_IsDialogueOpen || !m_IsClosingDialogue)
        {
            return;
        }

        m_HideTimer -= Time.deltaTime;
        if (m_HideTimer <= 0f)
        {
            HideDialogue();
        }
    }

    private void HideDialogue()
    {
        if (m_NonPlayerDialogue != null)
        {
            m_NonPlayerDialogue.style.display = DisplayStyle.None;
        }

        m_IsDialogueOpen = false;
        m_IsClosingDialogue = false;
    }

    private void StartFadeIn()
    {
        if (m_FadeOverlay == null)
        {
            return;
        }

        m_FadeOverlay.style.opacity = 1f;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return null;
        yield return null;

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Mathf.Min(Time.unscaledDeltaTime, MaxFadeDeltaTime);
            m_FadeOverlay.style.opacity = 1f - Mathf.Clamp01(timer / fadeInDuration);
            yield return null;
        }

        m_FadeOverlay.style.opacity = 0f;
    }
}

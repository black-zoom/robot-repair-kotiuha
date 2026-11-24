using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuHandler : MonoBehaviour
{
    private const string StartButtonName = "StartButton";
    private const string ExitButtonName = "ExitButton";
    private const string FadeOverlayName = "FadeOverlay";
    private const float MaxFadeDeltaTime = 0.05f;

    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Transitions")]
    public float fadeDuration = 0.35f;

    private Button m_StartButton;
    private Button m_ExitButton;
    private VisualElement m_FadeOverlay;
    private bool m_IsTransitioning;

    private void Start()
    {
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
        Time.timeScale = 1f;

        InitializeUI();
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    private void InitializeUI()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null || uiDocument.rootVisualElement == null)
        {
            Debug.LogError("MainMenuHandler: Missing UIDocument!");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        m_StartButton = root.Q<Button>(StartButtonName);
        m_ExitButton = root.Q<Button>(ExitButtonName);
        m_FadeOverlay = root.Q<VisualElement>(FadeOverlayName);

        RegisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        if (m_StartButton != null)
        {
            m_StartButton.clicked += OnStartClicked;
        }

        if (m_ExitButton != null)
        {
            m_ExitButton.clicked += OnExitClicked;
        }
    }

    private void UnregisterCallbacks()
    {
        if (m_StartButton != null)
        {
            m_StartButton.clicked -= OnStartClicked;
        }

        if (m_ExitButton != null)
        {
            m_ExitButton.clicked -= OnExitClicked;
        }
    }

    private void OnStartClicked()
    {
        if (m_IsTransitioning)
        {
            return;
        }

        m_IsTransitioning = true;
        StartCoroutine(TransitionToGame());
    }

    private void OnExitClicked()
    {
        if (m_IsTransitioning)
        {
            return;
        }

        m_IsTransitioning = true;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator TransitionToGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        if (asyncLoad == null)
        {
            m_IsTransitioning = false;
            yield break;
        }

        asyncLoad.allowSceneActivation = false;

        if (m_FadeOverlay != null && fadeDuration > 0f)
        {
            yield return null;

            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Mathf.Min(Time.unscaledDeltaTime, MaxFadeDeltaTime);
                m_FadeOverlay.style.opacity = Mathf.Clamp01(timer / fadeDuration);
                yield return null;
            }

            m_FadeOverlay.style.opacity = 1f;
        }

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}

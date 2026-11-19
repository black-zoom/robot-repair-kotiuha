using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public string gameSceneName = "GameScene";
    public float fadeDuration = 0.5f;

    private Button m_StartButton;
    private VisualElement m_FadeOverlay;
    private bool m_IsTransitioning;

    private void Start()
    {
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
        Time.timeScale = 1f;

        UIDocument uiDocument = GetComponent<UIDocument>();
        m_StartButton = uiDocument.rootVisualElement.Q<Button>("StartButton");
        m_FadeOverlay = uiDocument.rootVisualElement.Q<VisualElement>("FadeOverlay");

        if (m_StartButton != null)
        {
            m_StartButton.clicked += OnStartClicked;
        }
    }

    private void OnStartClicked()
    {
        if (m_IsTransitioning) return;
        m_IsTransitioning = true;

        StartCoroutine(TransitionToGame());
    }

    private IEnumerator TransitionToGame()
    {
        yield return null;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            if (m_FadeOverlay != null)
            {
                m_FadeOverlay.style.opacity = Mathf.Clamp01(timer / fadeDuration);
            }
            yield return null;
        }

        if (m_FadeOverlay != null)
        {
            m_FadeOverlay.style.opacity = 1f;
        }

        yield return null;
        SceneManager.LoadSceneAsync(gameSceneName);
    }
}

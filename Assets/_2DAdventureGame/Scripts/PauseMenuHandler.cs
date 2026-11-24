using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuHandler : MonoBehaviour
{
    private const string PauseMenuContainerName = "PauseMenuContainer";
    private const string ResumeButtonName = "ResumeButton";
    private const string ExitToMenuButtonName = "ExitToMenuButton";

    [Header("Input")]
    public InputAction pauseAction;

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    public bool IsPaused => m_IsPaused;

    private VisualElement m_PauseRoot;
    private Button m_ResumeButton;
    private Button m_ExitButton;

    private bool m_IsPaused;
    private bool m_IsExiting;

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePerformed;
        pauseAction.Disable();
    }

    private void Start()
    {
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
            Debug.LogError("PauseMenuHandler: Missing UIDocument!");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        m_PauseRoot = root.Q<VisualElement>(PauseMenuContainerName);
        m_ResumeButton = root.Q<Button>(ResumeButtonName);
        m_ExitButton = root.Q<Button>(ExitToMenuButtonName);

        SetPauseMenuVisible(false);
        RegisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        if (m_ResumeButton != null)
        {
            m_ResumeButton.clicked += ResumeGame;
        }

        if (m_ExitButton != null)
        {
            m_ExitButton.clicked += ExitToMainMenu;
        }
    }

    private void UnregisterCallbacks()
    {
        if (m_ResumeButton != null)
        {
            m_ResumeButton.clicked -= ResumeGame;
        }

        if (m_ExitButton != null)
        {
            m_ExitButton.clicked -= ExitToMainMenu;
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (m_IsExiting)
        {
            return;
        }

        if (m_IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        m_IsPaused = true;
        Time.timeScale = 0f;
        SetPauseMenuVisible(true);
    }

    private void ResumeGame()
    {
        m_IsPaused = false;
        Time.timeScale = 1f;
        SetPauseMenuVisible(false);
    }

    private void ExitToMainMenu()
    {
        if (m_IsExiting)
        {
            return;
        }

        m_IsExiting = true;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }

    private void SetPauseMenuVisible(bool isVisible)
    {
        if (m_PauseRoot != null)
        {
            m_PauseRoot.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}

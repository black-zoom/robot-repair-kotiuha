using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
    public InputAction pauseAction; 
    public string mainMenuSceneName = "MainMenu";

    private VisualElement m_PauseRoot;
    private Button m_ResumeButton;
    private Button m_ExitButton;
    private bool m_IsPaused = false;

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
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_PauseRoot = uiDocument.rootVisualElement.Q<VisualElement>("PauseMenuContainer");
        m_ResumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        m_ExitButton = uiDocument.rootVisualElement.Q<Button>("ExitToMenuButton");

        if (m_PauseRoot != null)
        {
            m_PauseRoot.style.display = DisplayStyle.None;
        }

        if (m_ResumeButton != null) m_ResumeButton.clicked += ResumeGame;
        if (m_ExitButton != null) m_ExitButton.clicked += ExitToMainMenu;
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (m_IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        m_IsPaused = true;
        Time.timeScale = 0f;
        
        if (m_PauseRoot != null) m_PauseRoot.style.display = DisplayStyle.Flex;
    }

    private void ResumeGame()
    {
        m_IsPaused = false;
        Time.timeScale = 1f;
        
        if (m_PauseRoot != null) m_PauseRoot.style.display = DisplayStyle.None;
    }

    private void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }
}

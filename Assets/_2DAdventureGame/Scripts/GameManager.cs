using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    public UIHandler uiHandler;

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";
    public float returnDelay = 3.0f;

    private EnemyController[] enemies;
    private bool gameOver;

    private void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        if (gameOver)
        {
            return;
        }

        if (player != null && player.health <= 0)
        {
            EndGame(true);
        }
        else if (AllEnemiesFixed())
        {
            EndGame(false);
        }
    }

    private void EndGame(bool isLose)
    {
        gameOver = true;
        Time.timeScale = 0f;

        if (uiHandler != null)
        {
            if (isLose)
            {
                uiHandler.DisplayLoseScreen();
            }
            else
            {
                uiHandler.DisplayWinScreen();
            }
        }

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSecondsRealtime(returnDelay);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }

    private bool AllEnemiesFixed()
    {
        if (enemies == null || enemies.Length == 0)
        {
            return false;
        }

        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null && enemy.isBroken)
            {
                return false;
            }
        }

        return true;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIHandler uiHandler;
    public string mainMenuSceneName = "MainMenu";
    public float returnDelay = 3.0f;

    EnemyController[] enemies;
    bool gameOver;

    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (gameOver)
            return;

        if (player.health <= 0)
        {
            EndGame(true);
        }
        else if (AllEnemiesFixed())
        {
            EndGame(false);
        }
    }

    void EndGame(bool isLose)
    {
        gameOver = true;
        Time.timeScale = 0f;

        if (isLose)
        {
            uiHandler.DisplayLoseScreen();
        }
        else
        {
            uiHandler.DisplayWinScreen();
        }

        StartCoroutine(ReturnToMainMenu());
    }

    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSecondsRealtime(returnDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    bool AllEnemiesFixed()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.isBroken)
                return false;
        }

        return true;
    }
}

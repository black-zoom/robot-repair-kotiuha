using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    EnemyController[] enemies;
    public UIHandler uiHandler;
    bool gameOver;

    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(
            FindObjectsSortMode.None);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (gameOver)
            return;

        if (player.health <= 0)
        {
            gameOver = true;
            uiHandler.DisplayLoseScreen();
            Invoke(nameof(ReloadScene), 3f);
        }

        if (AllEnemiesFixed())
        {
            gameOver = true;
            uiHandler.DisplayWinScreen();
            Invoke(nameof(ReloadScene), 3f);
        }
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

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public void Setup(int score)
    {
        gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;

        // Retrieves the current scene's name and reloads it
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    public void Setup(int score)
    {
        gameObject.SetActive(true);
    }

    public void NextButton()
    {
        Time.timeScale = 1f;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

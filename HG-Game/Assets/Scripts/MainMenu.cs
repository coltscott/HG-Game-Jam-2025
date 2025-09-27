using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }

    public void StartGame()
    {
        // TODO: Make it load the level the player was on isntead of level 1.
        SceneManager.LoadScene("Level1");
    }
}

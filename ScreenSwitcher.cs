using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Load Instructions Scene
    public void GoToInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    // Load Main Scene
    public void GoToMain()
    {
        SceneManager.LoadScene("Main");
    }
  public void GoToStart()
    {
        SceneManager.LoadScene("Start Screen");
    }

    // Optional: Quit Game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

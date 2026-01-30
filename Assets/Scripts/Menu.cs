using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject levelSelectMenu;

    public void LevelSelect()
    {
        startMenu.SetActive(false);
        levelSelectMenu.SetActive(true);
    }

    public void StartMenu()
    {
        startMenu.SetActive(true);
        levelSelectMenu.SetActive(false);
    }
    
    public void Level1()
    {
        SceneManager.LoadScene(1);
    }

    public void Level2()
    {
        SceneManager.LoadScene(2);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    
}
